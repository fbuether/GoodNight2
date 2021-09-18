using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GoodNight.Service.Storage.Journal
{
  internal class JournalWriter : IDisposable
  {
    private readonly BlockingCollection<Tuple<BaseRepository,Entry>> writeCache;

    private static readonly int WriteCacheSize = 64;

    private Task? writeCacheTask = null;

    private readonly CancellationTokenSource writeCacheCanceler;

    private Stream backingStore;

    private List<string> Exceptions = new List<string>();

    private DateTime lastWrite = DateTime.UnixEpoch;

    private readonly JsonSerializerOptions options;

    internal JournalWriter(JsonSerializerOptions options, Stream backingStore)
    {
      // uses a ConcurrentQueue by default.
      writeCache = new BlockingCollection<Tuple<BaseRepository,Entry>>(
        JournalWriter.WriteCacheSize);
      writeCacheCanceler = new CancellationTokenSource();
      this.backingStore = backingStore;

      // make a copy, just to be safe about multi-threading.
      this.options = new JsonSerializerOptions(options);
    }

    public void Dispose()
    {
      // if there is a stale exception from a previous run, throw it now.
      if (writeCacheTask is not null && writeCacheTask.Exception is not null)
      {
        throw writeCacheTask.Exception;
      }

      if (writeCacheTask is not null && !writeCacheTask.IsCompleted)
      {
        try
        {
          writeCacheCanceler.Cancel();
          writeCache.CompleteAdding();
          writeCacheTask.Wait();
        }
        catch (AggregateException ex)
        {
          Exceptions.Add(ex.Message);
          if (ex.InnerException is not null)
          {
            Exceptions.Add("Inner Exception: " + ex.InnerException.Message);
          }

          throw;
        }
      }

      ((IDisposable)writeCache).Dispose();
    }


    internal void StartWriting(Stream backingStore)
    {
      writeCacheTask = Task.Run(() =>
        this.DoWriteFromCache(writeCacheCanceler.Token));
    }

    /// <remarks>
    /// This runs on a separate thread, as started by StartWriting.
    /// </remarks>
    private void DoWriteFromCache(
      CancellationToken writeCanceler)
    {
      while (!writeCanceler.IsCancellationRequested || writeCache.Count > 0)
      {
        if (writeCache.Count > 0)
        {
          try
          {
            Write(writeCache.Take());
          }
          catch (Exception e)
          {
            Exceptions.Add(e.Message);
            throw;
          }
        }
        else
        {
          try
          {
            // wait for it...
            Write(writeCache.Take(writeCanceler));
          }
          catch (OperationCanceledException)
          {
            // don't do anything here. The while loop will quit automatically,
            // as the cancellation token is cancelled.
          }
          catch (Exception e)
          {
            Exceptions.Add(e.Message);
            throw;
          }
        }
      }
    }


    internal IEnumerable<string> GetExceptions()
    {
      foreach (var desc in Exceptions)
      {
        yield return desc;
      }
    }

    internal DateTime GetLastWrite()
    {
      return lastWrite;
    }

    internal void Queue(BaseRepository repos, Entry entry)
    {
      // if there is a stale exception from a previous run, throw it now.
      if (writeCacheTask is not null && writeCacheTask.Exception is not null)
      {
        throw writeCacheTask.Exception;
      }

      writeCache.Add(Tuple.Create(repos, entry));
    }

    private void Write(Tuple<BaseRepository, Entry> item)
    {
      var (repos,entry) = item;

      var writer = new Utf8JsonWriter(backingStore);
      writer.WriteStartObject();
      writer.WriteString("repos", repos.TypeName);

      switch (entry)
      {
        case Entry.Add a:
          writer.WriteString("kind", "Add");
          writer.WritePropertyName("value");
          JsonSerializer.Serialize(writer, a.Value, repos.ValueType, options);
          break;

        case Entry.Update u:
          writer.WriteString("kind", "Update");
          writer.WriteString("key", u.Key);
          writer.WritePropertyName("value");
          JsonSerializer.Serialize(writer, u.Value, repos.ValueType, options);
          break;

        case Entry.Delete d:
          writer.WriteString("kind", "Delete");
          writer.WriteString("key", d.Key);
          break;
      }

      writer.WriteEndObject();
      writer.Flush();
      lastWrite = DateTime.UtcNow;

      // write a newline.
      var breakWriter = new StreamWriter(backingStore);
      breakWriter.WriteLine();
      breakWriter.Flush();

      backingStore.Flush();
    }
  }
}

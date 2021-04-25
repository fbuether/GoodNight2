using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Storage.Journal;
using GoodNight.Service.Storage.Serialisation;

namespace GoodNight.Service.Storage
{
  public class Store : IStore, IDisposable
  {
    private Stream backingStore;

    private bool ownsBackingStore = false;

    private BlockingCollection<string> writeCache;

    private static readonly int WriteCacheSize = 64;

    private Task? writeCacheTask = null;

    private CancellationTokenSource writeCacheCanceler;

    private List<BaseRepository> repositories;

    internal JsonSerializerOptions JsonSerializerOptions { get; } =
      new JsonSerializerOptions();

    /// <param name="backingStore">
    /// A stream onto the backing store. The stream must be able to seek to
    /// start and end. If this stream already contains data, the data will
    /// be used to initialise this store. The store assumes exclusive access
    /// to the stream as well as the underlying data. It will dispose the stream
    /// when either this store or the repository is disposed.
    /// </param>
    public Store(IEnumerable<JsonConverter> converters,
      Stream? backingStore = null)
    {
      ownsBackingStore = backingStore is null;
      if (backingStore is null)
      {
        var cwd = Directory.GetCurrentDirectory();
        Console.WriteLine($"Store: Using {cwd}/store.json");
        this.backingStore = File.Open("store.json", FileMode.OpenOrCreate);
      }
      else {
        this.backingStore = backingStore;
      }

      repositories = new List<BaseRepository>();

      // uses a ConcurrentQueue by default.
      writeCache = new BlockingCollection<string>(Store.WriteCacheSize);
      writeCacheCanceler = new CancellationTokenSource();

      foreach (var converter in converters)
      {
        JsonSerializerOptions.Converters.Add(converter);
      }
    }

    public void Dispose()
    {
      if (writeCacheTask is not null && !writeCacheTask.IsCompleted)
      {
        writeCacheCanceler.Cancel();
        writeCache.CompleteAdding();
        writeCacheTask.Wait();
      }

      ((IDisposable)writeCache).Dispose();

      backingStore.Flush();

      if (ownsBackingStore)
      {
        ((IDisposable)backingStore).Dispose();
      }
    }


    /// <remarks>
    /// This method is not async, as it should be executed prior to web service
    /// launch, and should be executed synchronously.
    /// </remarks>
    public void StartJournal()
    {
      JournalReader.ReadAll(backingStore, this, JsonSerializerOptions);

      var writer = new StreamWriter(backingStore);
      writeCacheTask = Task.Run(() =>
        this.DoWriteFromCache(writer, writeCacheCanceler.Token));
    }

    private void DoWriteFromCache(StreamWriter writer,
      CancellationToken writeCanceler)
    {
      while (!writeCanceler.IsCancellationRequested || writeCache.Count > 0)
      {
        string? nextItem = null;
        bool success = false;

        if (writeCache.Count > 0)
        {
          nextItem = writeCache.Take();
          success = true;
        }
        else
        {
          try
          {
            // wait for it...
            success = writeCache.TryTake(out nextItem, Timeout.Infinite,
              writeCanceler);
          }
          catch (OperationCanceledException)
          {
            // don't do anything here. The while loop will quit automatically,
            // as the cancellation token is cancelled.
          }
        }

        if (!success || nextItem is null)
          continue;


        writer.WriteLine(nextItem);
      }

      // write out everything as we finished.
      writer.Flush();
    }

    // used by journal reader to find the appropriate repository for an entry.
    internal BaseRepository? Get(string uniqueName)
    {
      var existing = repositories.Find(repos => repos.UniqueName == uniqueName);
      if (existing is BaseRepository repos)
        return repos;

      return null;
    }

    IRepository<T> IStore.Create<T>(string uniqueName)
    {
      var existing = repositories.Find(repos => repos.UniqueName == uniqueName);
      if (existing is IRepository<T> existingRepos)
        return existingRepos;

      if (existing is not null && existing is not IRepository<T>)
        throw new Exception($"Invalid Repository creation. Unique Name "
          + $"\"{uniqueName}\" already exists, but not as repository for "
          + $"{nameof(T)}.");

      var repos = new Repository<T>(new JournalWriter(writeCache,
          JsonSerializerOptions), uniqueName);
      repositories.Add(repos);

      JsonSerializerOptions.Converters.Add(new ReferenceConverter<T>(this));

      return repos;
    }

    internal Repository<T>? GetRepository<T>(string uniqueName)
      where T : class, IStorable
    {
      return repositories.Find(repos => repos.UniqueName == uniqueName)
        as Repository<T>;
    }
  }
}

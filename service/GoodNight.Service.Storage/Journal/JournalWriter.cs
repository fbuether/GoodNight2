using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Concurrent;

namespace GoodNight.Service.Storage.Journal
{
  internal class JournalWriter
  {
    private BlockingCollection<string> writeCache;

    internal JournalWriter(BlockingCollection<string> writeCache)
    {
      this.writeCache = writeCache;
    }

    internal void QueueWrite(BaseRepository repos, Entry entry)
    {
      var stream = new MemoryStream();
      var writer = new Utf8JsonWriter(stream);

      writer.WriteStartObject();
      writer.WriteString("repos", repos.UniqueName);

      switch (entry)
      {
        case Entry.Add a:
          writer.WriteString("kind", "Add");
          writer.WritePropertyName("value");
          JsonSerializer.Serialize(writer, a.Value, repos.ValueType);
          break;

        case Entry.Update u:
          writer.WriteString("kind", "Update");
          writer.WritePropertyName("key");
          JsonSerializer.Serialize(writer, u.Key, repos.KeyType);
          writer.WritePropertyName("value");
          JsonSerializer.Serialize(writer, u.Value, repos.ValueType);
          break;

        case Entry.Delete d:
          writer.WriteString("kind", "Delete");
          writer.WritePropertyName("key");
          JsonSerializer.Serialize(writer, d.Key, repos.KeyType);
          break;
      }

      writer.WriteEndObject();
      writer.Flush();

      stream.Flush();
      stream.Seek(0, SeekOrigin.Begin);
      var reader = new StreamReader(stream, Encoding.UTF8);
      var asString = reader.ReadToEnd();

      writeCache.Add(asString);
    }
  }
}

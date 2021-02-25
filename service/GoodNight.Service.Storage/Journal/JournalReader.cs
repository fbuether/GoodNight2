using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace GoodNight.Service.Storage.Journal
{
  internal class JournalReader
  {
    private Stream stream;

    private StreamReader reader;

    internal JournalReader(Stream journalStream)
    {
      stream = journalStream;
      reader = new StreamReader(stream, Encoding.UTF8);
    }

    internal async IAsyncEnumerable<Entry> ReadAll()
    {
      stream.Seek(0, SeekOrigin.Begin);

      while (reader.EndOfStream)
      {
        var line = await reader.ReadLineAsync();
        if (line is null)
          yield break;

        var entry = JsonSerializer.Deserialize(line, typeof(Entry)) as Entry;
        if (entry is not null)
          yield return entry;
      }
    }
  }
}

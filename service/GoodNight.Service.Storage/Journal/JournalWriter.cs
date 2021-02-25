using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Threading.Tasks;
using System.Text;

namespace GoodNight.Service.Storage.Journal
{
  internal class JournalWriter
  {
    private Stream store;

    private StreamWriter writer;

    internal JournalWriter(Stream target)
    {
      store = target;

      writer = new StreamWriter(store, Encoding.UTF8);
    }

    internal async Task Write(Entry entry)
    {
      Console.WriteLine("--- serialising somethign.");
      Console.WriteLine("--- serialised: " + (JsonSerializer.Serialize(entry, entry.GetType())));
      await JsonSerializer.SerializeAsync(store, entry, entry.GetType());
      // await store.FlushAsync();
      await writer.WriteLineAsync();
      // await writer.WriteAsync("beeeeeeeeeeeeep");
      // new StreamWriter(this.store).WriteLine("newline.");
      await writer.FlushAsync();
      // await writer.WriteAsync(Environment.NewLine);
      await store.FlushAsync();

      // await 

      // var writer = new StreamWriter(store);
      // writer.Write("--done--");
      // Console.WriteLine("--- done.");
    }
  }
}

using System.Linq;
using System.Collections.Generic;
using System;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Storage.Journal;
using System.IO;
using System.Threading.Tasks;

namespace GoodNight.Service.Storage
{
  public class Store : IStore, IDisposable
  {
    private static string DefaultFilename = "store.json";

    private Stream backingStore;

    private Dictionary<Type, object> store = new Dictionary<Type, object>();

    private JournalWriter writer;

    public Store(Stream? journalStream = null)
    {
      backingStore = journalStream
        ?? File.OpenWrite(DefaultFilename);


      this.writer = new JournalWriter(backingStore);
    }

    public void Dispose()
    {
      ((IDisposable)backingStore).Dispose();
    }


    public static async Task Create(Stream? backingStore = null)
    {
      var store = new Store(backingStore);
      var loader = new JournalReader(store.backingStore);
      await store.Replay(loader.ReadAll());
    }

    private async Task Replay(IAsyncEnumerable<Entry> entries)
    {
      await foreach (var entry in entries)
      {
        switch (entry)
        {
          case Entry.Add a:
            // ugh.
            typeof(IStore).GetMethod("Add")!
              .MakeGenericMethod(new[] { a.ValueType, a.KeyType })
              .Invoke(this, new[] { a.Value });
            break;
        }
      }
    }


    private Dictionary<K, T> GetTypeDict<T,K>()
      where K : notnull
    {
      if (!store.TryGetValue(typeof(T), out var dict))
      {
        dict = new Dictionary<K,T>();
        store.Add(typeof(T), dict);
      }

      if (dict is Dictionary<K,T>)
      {
        return (Dictionary<K,T>)dict;
      }
      else
      {
        throw new Exception("Store Error: Invalid internal store entry.");
      }
    }


    async Task IStore.Add<T, K>(T element)
    {
      var dict = GetTypeDict<T,K>();
      dict[element.GetKey()] = element;
      await writer.Write(new Entry.Add(typeof(T), typeof(K), element.GetKey(), element));
    }


    T? IStore.Get<T, K>(K key)
      where T : class
    {
      var dict = GetTypeDict<T,K>();
      return dict.GetValueOrDefault(key);
    }


    IEnumerable<T> IStore.GetAll<T, K>()
    {
      var dict = GetTypeDict<T,K>();
      return dict.Values;
    }


    T? IStore.Update<T, K>(K key, Func<T, T> update) where T : class
    {
      var dict = GetTypeDict<T,K>();
      var oldElement = dict.GetValueOrDefault(key);
      if (oldElement is null) {
        return null;
      }

      var newElement = update(oldElement);
      dict[key] = newElement;
      return newElement;
    }


    U? IStore.WithUpdate<T, K, U>(K key, Func<T, (T, U)?> update)
      where U : class
    {
      var dict = GetTypeDict<T,K>();
      var oldElement = dict.GetValueOrDefault(key);
      if (oldElement is null) {
        return null;
      }

      var result = update(oldElement);
      if (result is null) {
        return null;
      }

      dict[key] = result.Value.Item1;
      return result.Value.Item2;
    }
    }
}

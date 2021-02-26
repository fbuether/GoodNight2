using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Storage.Journal;

namespace GoodNight.Service.Storage
{
  internal class Repository<T,K> : StoreBacked, IRepository<T,K>
    where T : class, IStorable<K>
    where K : notnull
  {
    private Dictionary<K,T> dict = new Dictionary<K,T>();

    internal Repository(Stream backingStore)
      : base(backingStore)
    {}

    internal override async Task ReadAll()
    {
      var reader = new JournalReader<T,K>(backingStore);
      await foreach (var entry in reader.ReadAll())
      {
        switch (entry)
        {
          case Entry<T,K>.Add a:
            this.Add(a.Value);
            break;
          case Entry<T,K>.Update u:
            this.Update(u.Key, (_) => u.Value);
            break;
          case Entry<T,K>.Delete d:
            this.Remove(d.Key);
            break;
        }
      }
    }


    public int Count => dict.Count;

    public IEnumerator<T> GetEnumerator()
    {
      return dict.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }


    public IStorableReference<T,K>? Add(T element)
    {
      var key = element.GetKey();
      if (dict.ContainsKey(key))
        return null;

      dict[key] = element;
      return new Reference<T,K>(this, key);
    }

    public T? Get(K key)
    {
      return dict.ContainsKey(key)
        ? dict[key]
        : null;
    }

    public T? Update(K key, Func<T, T?> update)
    {
      var oldElement = dict.GetValueOrDefault(key);
      if (oldElement is null)
        return null;

      var newElement = update(oldElement);
      if (newElement is null)
        return null;

      dict[key] = newElement;
      return newElement;
    }

    public U? Update<U>(K key, Func<T, (T, U)?> update)
      where U : class
    {
      var oldElement = dict.GetValueOrDefault(key);
      if (oldElement is null)
        return null;

      var result = update(oldElement);
      if (result is null)
        return null;

      dict[key] = result.Value.Item1;
      return result.Value.Item2;
    }

    public bool Remove(K key)
    {
      return dict.Remove(key);
    }
  }
}

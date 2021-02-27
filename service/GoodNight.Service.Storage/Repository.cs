using System;
using System.Collections;
using System.Collections.Generic;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Storage.Journal;

namespace GoodNight.Service.Storage
{
  internal class Repository<T,K> : BaseRepository, IRepository<T,K>
    where T : class, IStorable<K>
    where K : notnull
  {
    private Dictionary<K,T> dict = new Dictionary<K,T>();

    private JournalWriter writer;

    internal Repository(JournalWriter writer, string uniqueName)
      : base(uniqueName)
    {
      this.writer = writer;
    }


    // BaseRepository.

    internal override Type KeyType => typeof(K);

    internal override Type ValueType => typeof(T);


    // ReadOnlyDictionary.

    public int Count => dict.Count;

    public IEnumerator<T> GetEnumerator()
    {
      return dict.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }


    // IRepository.

    public IStorableReference<T,K>? Add(T element)
    {
      var key = element.GetKey();
      if (dict.ContainsKey(key))
        return null;

      dict[key] = element;


      writer.QueueWrite(new Entry.Add(UniqueName, element));
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
      writer.QueueWrite(new Entry.Update(UniqueName,
          key, result.Value.Item1));
      return result.Value.Item2;
      }

    public bool Remove(K key)
    {
      var contains = dict.Remove(key);
      if (contains)
      {
        writer.QueueWrite(new Entry.Delete(UniqueName, key));
      }

      return contains;
    }


    // Journaling.

    internal override bool Replay(Entry entry)
    {
      switch (entry)
      {
        case Entry.Add a:
          var addValue = (T)a.Value;
          if (addValue is null)
            return false;

          Add(addValue);
          return true;

        case Entry.Update u:
          var updKey = (K)u.Key;
          var updValue = (T)u.Value;
          if (updKey is null || updValue is null)
            return false;

          Update(updKey, _ => updValue);
          return true;

        case Entry.Delete d:
          var delKey = (K)d.Key;
          if (delKey is null)
            return false;

          Remove(delKey);
          return true;

        default:
          return false;
      }
    }
  }
}

using System;
using System.Collections;
using System.Collections.Generic;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Storage.Journal;

namespace GoodNight.Service.Storage
{
  internal class Repository<T> : BaseRepository, IRepository<T>
    where T : class, IStorable<T>
  {
    private Dictionary<string,T> dict = new Dictionary<string,T>();

    private JournalWriter writer;

    private bool writeUpdates = true;

    internal Repository(JournalWriter writer, string typeName)
      : base(typeName)
    {
      this.writer = writer;
    }


    // BaseRepository.

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

    public IReference<T>? Add(T element)
    {
      var key = element.Key;
      if (dict.ContainsKey(key))
        return null;

      dict[key] = element;

      if (writeUpdates)
      {
        writer.Queue(this, new Entry.Add(TypeName, element));
      }

      return new Reference<T>(this, key);
    }

    public T? Get(string key)
    {
      return dict.ContainsKey(key)
        ? dict[key]
        : null;
    }


    public IReference<T> GetReference(string key)
    {
      return new Reference<T>(this, key);
    }


    public IReference<T> Save(T element)
    {
      var reference = this.Update(element.Key, _ => element);
      return reference is null
        ? this.Add(element)! // if reference is null, adding will succeed.
        : reference;
    }

    private IReference<T>? Replace(string key, T? oldElement, T? newElement)
    {
      if (oldElement is null || newElement is null)
        return null;

      dict[key] = newElement;

      // if the element does not change, do not write this.
      if (writeUpdates && !newElement.Equals(oldElement))
      {
        writer.Queue(this, new Entry.Update(TypeName, key, newElement));
      }

      return new Reference<T>(this, key);
    }

    public IReference<T>? Update(string key, Func<T, T?> update)
    {
      var oldElement = dict.GetValueOrDefault(key);
      var newElement = oldElement is not null ? update(oldElement) : null;
      return Replace(key, oldElement, newElement);
    }


    public IReference<T>? Update(T element)
    {
      var oldElement = dict[element.Key];
      return Replace(element.Key, oldElement, element);
    }


    public U? Update<U>(string key, Func<T, (T, U)?> update)
      where U : class
    {
      var oldElement = dict.GetValueOrDefault(key);
      if (oldElement is null)
        return null;

      var result = update(oldElement);
      if (result is null)
        return null;

      var res = Replace(key, oldElement, result.Value.Item1);
      return res is not null ? result.Value.Item2 : null;
    }

    public bool Remove(string key)
    {
      var contains = dict.Remove(key);
      if (contains && writeUpdates)
      {
        writer.Queue(this, new Entry.Delete(TypeName, key));
      }

      return contains;
    }


    // Journaling.

    internal override bool Replay(Entry entry)
    {
      writeUpdates = false;

      try
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
            var updKey = u.Key;
            var updValue = (T)u.Value;
            if (updKey is null || updValue is null)
              return false;

            Update(updKey, _ => updValue);
            return true;

          case Entry.Delete d:
            var delKey = d.Key;
            if (delKey is null)
              return false;

            Remove(delKey);
            return true;

          default:
            return false;
        }
      }
      finally
      {
        writeUpdates = true;
      }
    }
  }
}

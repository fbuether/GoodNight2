using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Storage.Journal;

namespace GoodNight.Service.Storage
{
  internal class Repository<T> : BaseRepository, IRepository<T>
    where T : class, IStorable<T>
  {
    private Store store;

    private Dictionary<string,T> dict = new Dictionary<string,T>();

    private JournalWriter writer;

    private bool writeUpdates = true;

    internal Repository(Store store, JournalWriter writer, string typeName)
      : base(typeName)
    {
      this.store = store;
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
      var refedElement = UnlinkProperties(element);

      var key = refedElement.Key;
      if (dict.ContainsKey(key))
        return null;

      dict[key] = refedElement;

      if (writeUpdates)
      {
        writer.Queue(this, new Entry.Add(TypeName, refedElement));
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
      var refedElement = UnlinkProperties(element);
      var reference = this.Update(element.Key, _ => refedElement);
      return reference is null
        ? this.Add(refedElement)! // if reference is null, adding will succeed.
        : reference;
    }

    private IReference<T>? Replace(string key, T? oldElement, T? newElement)
    {
      if (oldElement is null || newElement is null)
        return null;

      var refedElement = UnlinkProperties(newElement);
      dict[key] = refedElement;

      // if the element does not change, do not write this.
      if (writeUpdates && !newElement.Equals(oldElement))
      {
        writer.Queue(this, new Entry.Update(TypeName, key, refedElement));
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


    /// <summary>
    /// Replaces all IReferences in this object with fresh, indirect references.
    /// This removes the risk of having stale references to old versions.
    /// </summary>
    /// <remarks>
    /// Sadly, this is not so elegant. But it does the job.
    /// </remarks>
    private T UnlinkProperties(T element)
    {
      // do not unlink during replay; read entries are always fully unlinked.
      if (!writeUpdates)
        return element;

      var streamStore = new MemoryStream();
      var writer = new Utf8JsonWriter(streamStore);

      // serialise
      JsonSerializer.Serialize(writer, element, ValueType,
        store.JsonSerializerOptions);
      writer.Flush();
      streamStore.Flush();

      streamStore.Seek(0, SeekOrigin.Begin);

      // deserialise
      var streamReader = new StreamReader(streamStore, Encoding.UTF8);
      var line = streamReader.ReadLine();
      if (line is null)
        throw new Exception($"Could not read serialised object \"{element}\".");

      var bytes = Encoding.UTF8.GetBytes(line);
      var reader = new Utf8JsonReader(bytes);
      var obj = JsonSerializer.Deserialize(ref reader, ValueType,
        store.JsonSerializerOptions) as T;

      if (obj is null)
        throw new Exception($"Could not read object from \"{line}\".");

      return obj;
    }
  }
}

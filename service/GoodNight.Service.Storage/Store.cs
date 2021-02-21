using System.Linq;
using System.Collections.Generic;
using System;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Storage
{
  public class Store : IStore
  {
    private Dictionary<Type, object> store = new Dictionary<Type, object>();

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


    void IStore.Add<T, K>(T element)
    {
      var dict = GetTypeDict<T,K>();
      var key = element.GetKey();
      if (dict.ContainsKey(key))
      {
        dict.Remove(key);
      }

      dict.Add(key, element);
    }

    T? IStore.Get<T, K>(K key)
      where T : class
    {
      var dict = GetTypeDict<T,K>();
      return dict.GetValueOrDefault(key);
    }

    IEnumerable<T> IStore.GetAll<T, K>()
    {
      throw new NotImplementedException();
    }

    T? IStore.Update<T, K>(K key, Func<T, T> update) where T : class
    {
      throw new NotImplementedException();
    }

    U? IStore.WithUpdate<T, K, U>(K key, Func<T, (T, U)?> update)
      where U : class
    {
      throw new NotImplementedException();
    }


    // public IEnumerable<T> GetAll<T>()
    //   where T : class
    // {
    //   return store
    //     .Where(obj => obj.Type is T)
    //     .Select(obj => obj.Data)
    //     .OfType<T>();
    // }

    // public void Add<T>(T newObj)
    //   where T : class
    // {
    //   var stored = new StoredObject(newObj);
    //   this.store.Add(stored);

    //   // todo: add all child elements to store.

    //   // todo: log adding this element.
    // }

    // public void Update<T>(T oldObj, T newObj)
    //   where T : class
    // {
    //   var stored = this.store.First(obj => obj.Data == oldObj);
    //   stored.Data = newObj;

    //   // todo: Update all elements pointing to this element, as well as their
    //   // transitive parents.

    //   // todo: log updating this element.
    // }

    // public void Remove<T>(T obj)
    //   where T : class
    // {
    //   var stored = this.store.First(obj => obj.Data == obj);
    //   this.store.Remove(stored);

    //   // todo: remove all child elements from store.

    //   // todo: log removing this element.
    // }

  }
}

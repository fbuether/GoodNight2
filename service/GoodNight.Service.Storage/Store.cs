using System.Linq;
using System.Collections.Generic;
using System;

namespace GoodNight.Service.Storage
{
  public class Store : IStore
  {
    private HashSet<StoredObject> store = new HashSet<StoredObject>();

    public Store()
    {
    }

    public IEnumerable<T> GetAll<T>()
      where T : class
    {
      return store
        .Where(obj => obj.Type is T)
        .Select(obj => obj.Data)
        .OfType<T>();
    }

    public void Add<T>(T newObj)
      where T : class
    {
      var stored = new StoredObject(newObj);
      this.store.Add(stored);

      // todo: add all child elements to store.

      // todo: log adding this element.
    }

    public void Update<T>(T oldObj, T newObj)
      where T : class
    {
      var stored = this.store.First(obj => obj.Data == oldObj);
      stored.Data = newObj;

      // todo: Update all elements pointing to this element, as well as their
      // transitive parents.

      // todo: log updating this element.
    }

    public void Remove<T>(T obj)
      where T : class
    {
      var stored = this.store.First(obj => obj.Data == obj);
      this.store.Remove(stored);

      // todo: remove all child elements from store.

      // todo: log removing this element.
    }
  }
}

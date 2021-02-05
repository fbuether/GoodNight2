using System.Linq;
using System.Collections.Generic;

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
        .Select(obj => obj.Data as T)
        .Where(obj => obj is not null);
    }
  }
}

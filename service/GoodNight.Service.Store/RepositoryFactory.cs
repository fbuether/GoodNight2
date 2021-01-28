using System.Collections.Generic;

namespace GoodNight.Service.Store
{
  public class RepositoryFactory
  {
    private Dictionary<string, object> repositories = new Dictionary<string, object>();

    public IRepository<T> Create<T>()
    {
      return new Repository<T>();
    }
  }
}

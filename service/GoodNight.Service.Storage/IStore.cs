using System.Collections.Generic;

namespace GoodNight.Service.Storage
{
  public interface IStore
  {
    public IEnumerable<T> GetAll<T>()
      where T : class;
  }
}

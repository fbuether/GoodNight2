using System;
using System.Collections.Immutable;

namespace GoodNight.Service.Store
{
  public interface IRepository<T>
  {
    public IImmutableSet<T> Get();

    public void Save(T value);

    public void Update(T value, Func<T, T> update);

    public void Delete(T value);
  }
}

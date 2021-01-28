using System;
using System.Collections.Immutable;

namespace GoodNight.Service.Store
{
  internal class Repository<T> : IRepository<T>
  {
    public IImmutableSet<T> Get()
    {
      System.Console.WriteLine("Get()");
      throw new NotImplementedException();
    }

    public void Save(T value)
    {
      System.Console.WriteLine("Save()");
      throw new NotImplementedException();
    }

    public void Update(T value, Func<T, T> update)
    {
      System.Console.WriteLine("Update()");
      throw new NotImplementedException();
    }

    public void Delete(T value)
    {
      System.Console.WriteLine("Delete()");
      throw new NotImplementedException();
    }
  }
}


using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Storage
{
  internal class Reference<T> : IReference<T>
    where T : class, IStorable<T>
  {
    public string Key { get; init; }

    internal Repository<T> Repository { get; init; }

    internal Reference(Repository<T> repository, string key)
    {
      Repository = repository;
      Key = key;
    }

    public T? Get()
    {
      return Repository.Get(Key);
    }

    public string GetTag()
    {
      return Repository.TypeName;
    }

    public override string ToString()
    {
      return $"IReference{{repos:\"{typeof(T).Name}\", key:\"{Key}\"}}";
    }
  }
}


using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Storage
{
  internal record Reference<T>(Repository<T> Repository, string Key)
    : IReference<T>
    where T : class, IStorable<T>
  {
    public T? Get()
    {
      return Repository.Get(Key);
    }

    internal string GetTag()
    {
      return Repository.TypeName;
    }

    public override string ToString()
    {
      return $"IReference{{repos:\"{typeof(T).Name}\", key:\"{Key}\"}}";
    }
  }
}

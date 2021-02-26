
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Storage
{
  internal class Reference<T,K> : IStorableReference<T,K>
    where T : class, IStorable<K>
    where K : notnull
  {
    private IRepository<T,K> repository;

    public K Key { get; init; }

    internal Reference(IRepository<T,K> repository, K key)
    {
      this.repository = repository;
      Key = key;
    }

    public T? Get()
    {
      return repository.Get(Key);
    }
  }
}

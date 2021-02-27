using System.Collections.Generic;
using System;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Storage.Journal;
using System.IO;
using System.Threading.Tasks;

namespace GoodNight.Service.Storage
{
  public class Store : IStore, IDisposable
  {
    private Stream backingStore;

    private List<BaseRepository> repositories;

    /// <param name="backingStore">
    /// A stream onto the backing store. The stream must be able to seek to
    /// start and end. If this stream already contains data, the data will
    /// be used to initialise this store. The store assumes exclusive access
    /// to the stream as well as the underlying data. It will dispose the stream
    /// when either this store or the repository is disposed.
    /// </param>
    public Store(Stream? backingStore = null)
    {
      this.backingStore = backingStore is null
        ? File.Open("store.json", FileMode.OpenOrCreate)
        : backingStore;
      repositories = new List<BaseRepository>();
    }

    public void Dispose()
    {
      ((IDisposable)backingStore).Dispose();
    }

    public void LoadAll()
    {
      JournalReader.ReadAll(backingStore, this);
    }

    // used by journal reader to find the appropriate repository for an entry.
    internal BaseRepository? Get(string uniqueName)
    {
      var existing = repositories.Find(repos => repos.UniqueName == uniqueName);
      if (existing is BaseRepository repos)
        return repos;

      return null;
    }

    IRepository<T,K> IStore.Create<T,K>(string uniqueName)
    {
      var existing = repositories.Find(repos => repos.UniqueName == uniqueName);
      if (existing is IRepository<T,K> existingRepos)
        return existingRepos;

      if (existing is not null && existing is not IRepository<T,K>)
        throw new Exception($"Invalid Repository creation. Unique Name "
          + $"\"{uniqueName}\" already exists, but not as repository for "
          + $"{nameof(T)},{nameof(K)}.");

      var repos = new Repository<T,K>(new JournalWriter(), uniqueName);
      repositories.Add(repos);
      return repos;
    }

    Task IStore.Sync()
    {
      throw new NotImplementedException();
    }
  }
}

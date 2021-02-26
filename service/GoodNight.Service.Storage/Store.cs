using System.Linq;
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
    private List<StoreBacked> repositories;

    public Store()
    {
      repositories = new List<StoreBacked>();
    }

    public void Dispose()
    {
      foreach (var repos in repositories)
      {
        ((IDisposable)repos).Dispose();
      }
    }


    public IRepository<T, K> Create<T, K>(Stream backingStore)
      where T : class, IStorable<K>
      where K : notnull
    {
      var repos = new Repository<T,K>(backingStore);
      repositories.Add(repos);


      return repos;
    }

    public IRepository<T, K> Create<T, K>(string uniqueName)
      where T : class, IStorable<K>
      where K : notnull
    {
      var stream = File.Open($"store-{uniqueName}.json", FileMode.OpenOrCreate);
      return Create<T,K>(stream);
    }


    public async Task LoadAll()
    {
      foreach (var repos in repositories)
      {
        await repos.ReadAll();
      }
    }



    public Task Sync()
    {
      throw new NotImplementedException();
    }
  }
}

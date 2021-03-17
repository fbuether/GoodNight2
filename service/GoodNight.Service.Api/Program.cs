using System;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using GoodNight.Service.Storage.Interface;
using System.Threading.Tasks;
using GoodNight.Service.Storage;
using GoodNight.Service.Api.Storage;

namespace GoodNight.Service.Api
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var host = Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webHostBuilder =>
        {
          webHostBuilder.UseStartup<Startup>();
        })
        .Build();

      var store = host.Services.GetService(typeof(IStore)) as Store;
      if (store is null)
      {
        throw new NullReferenceException();
      }

      // instantiate all stores.
      host.Services.GetService(typeof(ReadStore));
      host.Services.GetService(typeof(WriteStore));

      store.StartJournal();

      Console.WriteLine("Finished loading stores. Starting host...");

      // var readStore = await ReadStore.Initialise(store);
      // var loadStores = await host.Services.GetService(

      host.Run();
    }
  }
}

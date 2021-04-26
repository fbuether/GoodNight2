using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GoodNight.Service.Api.Storage;
using GoodNight.Service.Storage;
using GoodNight.Service.Storage.Interface;

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

      var store = host.Services.GetService<IStore>() as Store;
      if (store is null)
      {
        throw new NullReferenceException();
      }

      // instantiate all stores, in order to have them available when
      // `store.StartJournal` reads the whole journal.
      host.Services.GetService<ReadStore>();
      host.Services.GetService<WriteStore>();

      // Start reading the stored journal.
      store.StartJournal();

      var proc = System.Diagnostics.Process.GetCurrentProcess();
      proc.Refresh();
      var usage = Math.Truncate(proc.WorkingSet64 / 1048576.0);

      Console.WriteLine("Finished loading stores.");
      Console.WriteLine($"Memory usage: {usage}m");
      Console.WriteLine("Starting host...");
      host.Run();
    }
  }
}

using System;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

namespace GoodNight.Service.Api
{
  public class Program
  {
    public static void Main(string[] args)
    {
      Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webHostBuilder =>
        {
          webHostBuilder.UseStartup<Startup>();
        })
        .Build()
        .Run();
    }
  }
}

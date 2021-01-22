using System;
using Microsoft.Extensions.Hosting;

namespace GoodNight.Service.Api
{
  public class GoodNight
  {
    public static void Main(string[] args)
    {
      Host.CreateDefaultBuilder();


      System.Console.WriteLine("hello, now.");
    }
  }
}

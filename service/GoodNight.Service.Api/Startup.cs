using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using GoodNight.Service.Storage;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Api.Converter;
using GoodNight.Service.Api.Storage;

namespace GoodNight.Service.Api
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllers()
        .AddJsonOptions(options => {
          options.JsonSerializerOptions.Converters.Add(
            new ActionChoiceConverter());
          options.JsonSerializerOptions.Converters.Add(
            new ExpressionValueConverter());
        });

      services.AddCors(corsOptions => {
        corsOptions.AddDefaultPolicy(builder => {
          builder
            .WithHeaders(new[] {
                "Content-Type"
              })
            .WithOrigins("http://localhost:32015",
            "https://goodnight.jasminefields.net");
        });
      });

      services.AddSingleton<IStore, Store>();
      services.AddSingleton<ReadStore>();
      services.AddSingleton<WriteStore>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.EnvironmentName == "Development")
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseRouting();

      app.UseCors();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using GoodNight.Service.Storage;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Api.Converter;
using GoodNight.Service.Api.Storage;
using System.Text.Json;

namespace GoodNight.Service.Api
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      var jsonOptions = new JsonSerializerOptions();
      jsonOptions.Converters.Add(new ActionChoiceConverter());
      jsonOptions.Converters.Add(new ExpressionValueConverter());
      jsonOptions.Converters.Add(new WriteQualityConverter());
      services.AddSingleton<JsonSerializerOptions>(jsonOptions);

      services.AddControllers()
        .AddJsonOptions(options => {
          foreach (var converter in jsonOptions.Converters)
          {
            options.JsonSerializerOptions.Converters.Add(converter);
          }
        });

      services.AddCors(corsOptions => {
        corsOptions.AddDefaultPolicy(builder => {
          builder
            .WithHeaders(new[] {
                "Content-Type"
              })
            .WithOrigins("http://localhost:32015",
              "https://goodnight.jasminefields.net")
            .WithMethods("GET", "PUT", "POST", "DELETE");
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

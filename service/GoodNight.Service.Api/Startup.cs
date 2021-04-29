using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using GoodNight.Service.Api.Converter;
using GoodNight.Service.Storage;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Domain.Serialisation.Read;

namespace GoodNight.Service.Api
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      var mvcBuilder = services.AddControllers();

      services.Configure<JsonOptions>(options =>
      {
        var converters = options.JsonSerializerOptions.Converters;
        converters.Add(new ActionChoiceConverter());
        converters.Add(new ExpressionValueConverter());
        converters.Add(new WriteQualityConverter());
        converters.Add(new SceneContentConverter());
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

      services.AddSingleton<IStore, Store>((IServiceProvider services) =>
      {
        var options = services.GetService<IOptions<JsonOptions>>();
        if (options is null)
          throw new Exception();

        return new Store(options.Value.JsonSerializerOptions.Converters);
      });
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

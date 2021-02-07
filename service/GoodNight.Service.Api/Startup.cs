using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using GoodNight.Service.Storage;
using GoodNight.Service.Api.Converter;

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
          options.JsonSerializerOptions.PropertyNamingPolicy = null;
        });

      services.AddSingleton<IStore, Store>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.EnvironmentName == "Development")
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseRouting();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}

using System;
using CompressedStaticFiles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using GoodNight.Service.Domain.Serialisation.Expressions;
using GoodNight.Service.Domain.Serialisation.Read;
using GoodNight.Service.Storage;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Api
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      var mvcBuilder = services.AddControllers();

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

      services.Configure<JsonOptions>(options =>
      {
        var converters = options.JsonSerializerOptions.Converters;
        converters.Add(new ExpressionValueConverter());
        converters.Add(new LogChoiceConverter());
        converters.Add(new QualityConverter());
        converters.Add(new SceneContentConverter());
        converters.Add(new ExpressionConverterFactory());
        converters.Add(new PlayerConverter());
      });


      services.AddSingleton<IStore, Store>((IServiceProvider services) =>
      {
        var options = services.GetService<IOptions<JsonOptions>>();
        if (options is null)
          throw new Exception();

        return new Store(options.Value.JsonSerializerOptions.Converters);
      });

      services.AddCompressedStaticFiles();
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

      // everything not found is treated like index.html.
      // app.UseStatusCodePagesWithReExecute("/index.html");

      // Serve static content, and serve index.html on / requests.
      app.UseDefaultFiles();
      app.UseCompressedStaticFiles();
    }
  }
}

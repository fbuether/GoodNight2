using System;
using CompressedStaticFiles;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using GoodNight.Service.Api.Authentication;
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

      // authentication either via JWT, or via TemporaryUserAuthentication.
      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Audience = "d8ee60f3-f059-4169-93b4-8faf1c32a9d8";
            options.Authority = "https://login.microsoftonline.com/9188040d-6c67-4c5b-b112-36a304b66dad/v2.0";
        })
        .AddScheme<TemporaryUserAuthenticationHandler.NoOptions,
        TemporaryUserAuthenticationHandler>(
          TemporaryUserAuthenticationHandler.AuthenticationScheme,
          options => {});

      services.AddAuthorization(options =>
      {
        var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
          JwtBearerDefaults.AuthenticationScheme,
          TemporaryUserAuthenticationHandler.AuthenticationScheme);
        defaultAuthorizationPolicyBuilder =
          defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
        options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
      });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.EnvironmentName == "Development")
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseCors();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });

      // everything not found is treated like index.html.
      app.UseStatusCodePagesWithReExecute("/index.html");

      // Serve static content, and serve index.html on / requests.
      app.UseDefaultFiles();
      app.UseCompressedStaticFiles();
    }
  }
}

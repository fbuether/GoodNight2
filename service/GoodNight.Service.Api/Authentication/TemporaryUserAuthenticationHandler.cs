using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GoodNight.Service.Api.Authentication
{
  public class TemporaryUserAuthenticationHandler
    : AuthenticationHandler<TemporaryUserAuthenticationHandler.NoOptions>
  {
    public class NoOptions : AuthenticationSchemeOptions { }

    public static readonly string AuthenticationScheme = "TemporaryUser";

    public TemporaryUserAuthenticationHandler(
      IOptionsMonitor<NoOptions> options,
      ILoggerFactory logger,
      UrlEncoder encoder,
      ISystemClock clock)
      : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
      if (!Request.Headers.ContainsKey("Authorization"))
        return Task.FromResult(AuthenticateResult.Fail(
            "No Authorization header found."));

      var auth = Request.Headers["Authorization"].ToString();
      if (!auth.StartsWith("Guest "))
        return Task.FromResult(AuthenticateResult.Fail(
            "No Temporary value in Authorization header."));

      var name = auth.Substring("Guest ".Count());

      Guid temporary;
      if (!Guid.TryParse(name, out temporary))
        return Task.FromResult(AuthenticateResult.Fail(
            "Invalid name for temporary user authentication."));

      return Task.FromResult(
        AuthenticateResult.Success(
          new AuthenticationTicket(
            new ClaimsPrincipal(
              new ClaimsIdentity(
                new [] { new Claim(ClaimTypes.NameIdentifier, name) },
                nameof(TemporaryUserAuthenticationHandler))),
            Scheme.Name)));
    }
  }
}

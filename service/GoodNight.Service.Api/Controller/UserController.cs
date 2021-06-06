using GoodNight.Service.Api.Controller.Base;
using GoodNight.Service.Storage.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodNight.Service.Api.Controller
{
  [ApiController]
  [Route("api/v1/user")]
  public class UserController : AuthorisedController
  {
    public UserController(IStore store)
      : base(store)
    {
    }

    public record MergeableUser(string auth);

    [HttpPost("/merge")]
    public ActionResult MergeUser(MergeableUser previous)
    {
      // todo: merge previous into the current user
      // todo: update database to replace all user identity with new user
      return this.Conflict();
    }
  }
}

using System;
using System.Security.Claims;
using GoodNight.Service.Domain.Model;
using GoodNight.Service.Storage.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodNight.Service.Api.Controller.Base
{
  [Authorize]
  [ApiController]
  public abstract class AuthorisedController : ControllerBase
  {
    private IRepository<User> users;

    internal AuthorisedController(IStore store)
    {
      this.users = store.Create<User>();
    }

    protected User GetCurrentUser()
    {
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
      if (userIdClaim is null)
        throw new Exception("Asp.Net did not supply name identifier.");

      var userId = userIdClaim.Value;
      var user = users.Get(userId);

      if (user is null)
      {
        user = Domain.Model.User.Create(userId);
        users.Add(user);
      }

      return user;
    }

    protected bool IsAuthorised()
    {
      return GetCurrentUser() is not null;
    }
  }
}

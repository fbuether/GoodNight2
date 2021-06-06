using System;
using System.Linq;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GoodNight.Service.Domain.Model;
using GoodNight.Service.Domain.Model.Read;
using GoodNight.Service.Storage.Interface;
using TransferAdventure = GoodNight.Service.Domain.Model.Read.Transfer.Adventure;
using GoodNight.Service.Api.Controller.Base;

namespace GoodNight.Service.Api.Controller.Read
{
  [ApiController]
  [Route("api/v1/read/stories/{storyUrlname}/")]
  public class ReadStoryController : AuthorisedController
  {
    private IRepository<Adventure> adventures;
    private IRepository<User> users;
    private IRepository<Story> stories;

    public ReadStoryController(IStore store)
      : base(store)
    {
      adventures = store.Create<Adventure>();
      users = store.Create<User>();
      stories = store.Create<Story>();
    }

    public record AdventureStart(string name);

    [HttpPost("start")]
    public ActionResult<TransferAdventure> StartAdventure(string storyUrlname,
      [FromBody] AdventureStart start)
    {
      var story = stories.Get(storyUrlname);
      if (story is null)
        return NotFound();

      if (start.name.Trim() == "")
        return BadRequest(new ErrorResult(
            "The player name must not be empty."));

      return GetCurrentUser()
        .StartAdventure(story, start.name.Trim())
        .Do(ua => users.Save(ua.Item1))
        .Map<ActionResult<TransferAdventure>>(
          ua => Ok(ua.Item2.ToTransfer(true)),
          err => BadRequest(new ErrorResult(err)));
    }


    [HttpGet("continue")]
    public ActionResult<TransferAdventure> GetAdventure(string storyUrlname)
    {
      var story = stories.Get(storyUrlname);
      if (story is null)
        return NotFound();

      var adventure = GetCurrentUser().GetAdventure(storyUrlname);
      if (adventure is null)
        return BadRequest(new ErrorResult("User has not started Adventure."));

      return Ok(adventure.ToTransfer(true));
    }


    public record Choice(string choice);

    [HttpPost("do")]
    public ActionResult<TransferAdventure> DoOption(string storyUrlname,
      [FromBody] Choice choice)
    {
      var story = stories.Get(storyUrlname);
      if (story is null)
        return NotFound();

      var optionname = choice.choice;
      if (String.IsNullOrEmpty(optionname))
        return BadRequest(new ErrorResult("No Option given."));

      var adventure = users.Update(GetCurrentUser().Key, user =>
        user.ContinueAdventure(story, optionname));
      if (adventure is null)
        return BadRequest(new ErrorResult("Option not found or not valid now."));

      return Ok(adventure.ToTransfer(false));
    }


    [HttpDelete]
    public ActionResult DeleteAdventure(string storyUrlname)
    {
      var story = stories.Get(storyUrlname);
      if (story is null)
        return NotFound();

      var user = GetCurrentUser();
      var adventure = user.GetAdventure(storyUrlname);
      if (adventure is null)
        return BadRequest(new ErrorResult("User has not started Adventure."));

      adventures.Remove(adventure.Key);
      user = user.RemoveAdventure(adventure);
      users.Save(user);

      return NoContent();
    }
  }
}

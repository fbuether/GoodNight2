using System;
using System.Linq;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GoodNight.Service.Domain.Model;
using GoodNight.Service.Domain.Model.Read;
using GoodNight.Service.Storage.Interface;
using TransferAdventure = GoodNight.Service.Domain.Model.Read.Transfer.Adventure;
using TransferConsequence = GoodNight.Service.Domain.Model.Read.Transfer.Consequence;

namespace GoodNight.Service.Api.Controller.Read
{
  [ApiController]
  [Route("api/v1/read/stories/{storyUrlname}/")]
  public class ReadStoryController : ControllerBase
  {
    private IRepository<Adventure> adventures;
    private IRepository<User> users;
    private IRepository<Story> stories;

    public ReadStoryController(IStore store)
    {
      adventures = store.Create<Adventure>();
      users = store.Create<User>();
      stories = store.Create<Story>();
    }


    private User? GetCurrentUser()
    {
      var key = Guid.Empty;

      var user = users.Get(key.ToString());
      if (user is null)
        // return Unauthorized();
        // todo: replace with commented code above.
        user = new User(key, "current-user-name", "e@mail",
          ImmutableHashSet.Create<IReference<Adventure>>());

      return user;
    }


    public record AdventureStart(string name);

    [HttpPost("start")]
    public ActionResult<TransferAdventure> StartAdventure(string storyUrlname,
      [FromBody] AdventureStart start)
    {
      var user = GetCurrentUser();
      if (user is null)
        return Unauthorized();

      var story = stories.Get(storyUrlname);
      if (story is null)
        return NotFound();

      return user.StartAdventure(story, start.name)
        .Do(ua => users.Save(ua.Item1))
        .Do(ua => adventures.Save(ua.Item2))
        .Map<ActionResult<TransferAdventure>>(
          ua => Ok(ua.Item2.ToTransfer()),
          err => BadRequest(new ErrorResult(err)));
    }


    [HttpGet("continue")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TransferAdventure> GetAdventure(string storyUrlname)
    {
      var user = GetCurrentUser();
      if (user is null)
        return Unauthorized();

      var story = stories.Get(storyUrlname);
      if (story is null)
        return NotFound();

      var adventure = user.GetAdventure(storyUrlname);
      if (adventure is null)
        return BadRequest(new ErrorResult("User has not started Adventure."));

      return Ok(adventure.ToTransfer());
    }


    public record Choice(string choice);

    [HttpPost("do")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TransferConsequence> DoOption(string storyUrlname,
      [FromBody] Choice choice)
    {
      var username = "current-user-name";

      var user = users.Get(username);
      if (user is null)
        return Unauthorized();

      var story = stories.Get(storyUrlname);
      if (story is null)
        return NotFound();

      var optionname = choice.choice;
      if (String.IsNullOrEmpty(optionname))
        return BadRequest(new ErrorResult("No Option given."));

      var consequence = users.Update(username, user =>
        user.ContinueAdventure(story, optionname));
      if (consequence is null)
        return BadRequest("Option not found or not valid now.");

      return Ok(consequence);
    }
  }
}

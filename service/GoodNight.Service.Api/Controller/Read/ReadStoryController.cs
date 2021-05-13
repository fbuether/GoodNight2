using System;
using System.Linq;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GoodNight.Service.Domain;
using GoodNight.Service.Domain.Model;
using GoodNight.Service.Domain.Model.Read;
using GoodNight.Service.Storage.Interface;

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


    [HttpGet("continue")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Adventure> GetAdventure(string storyUrlname)
    {
      var username = "current-user-name";

      var user = users.Get(username);
      if (user is null)
        // return Unauthorized();
        // todo: replace with commented code above.
        user = new User(Guid.Empty, "current-user-name", "e@mail",
          ImmutableHashSet.Create<IReference<Adventure>>());

      var story = stories.Get(storyUrlname);
      if (story is null)
        return NotFound();

      var advKey = NameConverter.Concat(user.Key, story.Key);
      var adventure = user.Adventures.FirstOrDefault(a => a.Key == advKey);
      if (adventure is null)
        return BadRequest(new ErrorResult("User has not started Adventure."));

      return Ok(adventure);
    }


    public record Choice(string choice);

    [HttpPost("do")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Consequence> DoOption(string storyUrlname,
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

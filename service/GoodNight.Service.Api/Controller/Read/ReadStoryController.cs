using System;
using System.Linq;
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
    private IRepository<Log> logs;

    public ReadStoryController(IStore store)
    {
      adventures = store.Create<Adventure>();
      users = store.Create<User>();
      stories = store.Create<Story>();
      logs = store.Create<Log>();
    }


    [HttpGet("continue")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Adventure> GetAdventure(string storyUrlname)
    {
      var username = "current-user-name";

      var user = users.Get(username);
      if (user is null)
        return Unauthorized("Authentication not found or invalid.");

      var story = stories.Get(storyUrlname);
      if (story is null)
        return NotFound("Story not found.");

      var advKey = NameConverter.Concat(user.GetKey(), story.GetKey());
      var adventure = user.Adventures.First(a => a.Key == advKey);
      if (adventure is null)
        return Forbid("User has not started Adventure.");

      return Ok(adventure);
    }

    [HttpPost("do")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Consequence> DoOption(string storyUrlname,
      [FromBody] string optionname)
    {
      var username = "current-user-name";

      if (String.IsNullOrEmpty(optionname))
        return BadRequest("No Option given.");

      var user = users.Get(username);
      if (user is null)
        return Unauthorized("Authentication not found or invalid.");

      var story = stories.Get(storyUrlname);
      if (story is null)
        return NotFound("Story not found.");

      var consequence = users.Update(username, (User user) =>
        user.ContinueAdventure(adventures, logs, story, optionname));

      if (consequence is null)
        return BadRequest("Option not found or not valid now.");

      return Ok(consequence);
    }
  }
}

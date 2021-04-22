using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GoodNight.Service.Api.Storage;
using GoodNight.Service.Domain.Model.Read;
using GoodNight.Service.Domain.Model;

namespace GoodNight.Service.Api.Read
{
  [ApiController]
  [Route("api/v1/read/stories/{storyUrlname}/")]
  public class ReadStoryController : ControllerBase
  {
    private ReadStore store;

    public ReadStoryController(ReadStore store)
    {
      this.store = store;
    }


    [HttpGet("continue")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Adventure> GetAdventure(string storyUrlname)
    {
      var username = "current-user-name";

      var user = store.Users.Get(username);
      if (user == null)
        return Unauthorized("Authentication not found or invalid.");

      var story = store.Stories.Get(storyUrlname);
      if (story == null)
        return NotFound("Story not found.");

      var adventure = user.Adventures.First(a => a.Story.Key == storyUrlname);
      if (adventure == null)
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

      var user = store.Users.Get(username);
      if (user == null)
        return Unauthorized("Authentication not found or invalid.");

      var story = store.Stories.Get(storyUrlname);
      if (story == null)
        return NotFound("Story not found.");

      var consequence = store.Users.Update(username, (User user) =>
        user.ContinueAdventure(story, optionname));

      if (consequence == null)
        return BadRequest("Option not found or not valid now.");

      return Ok(consequence);
    }
  }
}

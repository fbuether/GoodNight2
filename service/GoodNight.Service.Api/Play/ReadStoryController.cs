using GoodNight.Service.Domain.Model.Read;
using GoodNight.Service.Domain.Model;
using GoodNight.Service.Storage.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;

namespace GoodNight.Service.Api.Play
{
  [ApiController]
  [Route("api/v1/read")]
  public class ReadStoryController : ControllerBase
  {
    private IStore store;

    public ReadStoryController(IStore store)
    {
      this.store = store;
    }


    [HttpGet("{storyname}/continue")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Adventure> GetAdventure(string storyname)
    {
      var username = "current-user-name";

      var user = store.Get<User, string>(username);
      if (user == null)
        return Unauthorized("Authentication not found or invalid.");

      var story = store.Get<Story, string>(storyname);
      if (story == null)
        return NotFound("Story not found.");

      var adventure = user.Adventures.First(a => a.Story.Key == storyname);
      if (adventure == null)
        return Forbid("User has not started Adventure.");

      return Ok(adventure);
    }

    [HttpPost("{storyname}/do")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Consequence> DoOption(string storyname,
      [FromBody] string optionname)
    {
      var username = "current-user-name";

      if (String.IsNullOrEmpty(optionname))
        return BadRequest("No Option given.");

      var user = store.Get<User, string>(username);
      if (user == null)
        return Unauthorized("Authentication not found or invalid.");

      var story = store.Get<Story, string>(storyname);
      if (story == null)
        return NotFound("Story not found.");

      var consequence = store.WithUpdate(username, (User user) =>
        user.ContinueAdventure(story, optionname));

      if (consequence == null)
        return BadRequest("Option not found or not valid now.");

      return Ok(consequence);
    }
  }
}

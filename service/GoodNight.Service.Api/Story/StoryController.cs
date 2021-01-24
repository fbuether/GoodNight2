using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace GoodNight.Service.Api.Story
{
  [ApiController]
  [Route("api/v1/stories")]
  public class StoryController : ControllerBase
  {
    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public ActionResult<string> Get()
    {
      var story = new Domain.Story.Story("Helms Schlund");

      System.Console.WriteLine(story);

      return Ok("hello");
    }
  }
}

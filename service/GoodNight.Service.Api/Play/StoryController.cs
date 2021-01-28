using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using GoodNight.Service.Domain.Write;

namespace GoodNight.Service.Api.Play
{
  [ApiController]
  [Route("api/v1/stories")]
  public class StoryController : ControllerBase
  {
    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public ActionResult<string> Get()
    {
      var story = Story.Create("Helms Schlund");

      System.Console.WriteLine(story);

      return Ok("hello");
    }
  }
}

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using GoodNight.Service.Domain.Read;
using GoodNight.Service.Storage;
using System.Collections.Generic;

namespace GoodNight.Service.Api.Play
{
  [ApiController]
  [Route("api/v1/stories")]
  public class StoryController : ControllerBase
  {
    private IStore store;

    public StoryController(IStore store)
    {
      this.store = store;
    }

    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status200OK,
      Type = typeof(IEnumerable<Story>))]
    public ActionResult<IEnumerable<Story>> Get()
    {
      return Ok(new Story[] {});

      // var story = Story.Create("Helms Schlund");

      // System.Console.WriteLine(story);

      // return Ok("hello");
    }
  }
}

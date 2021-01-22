using System;
using Microsoft.AspNetCore.Mvc;

namespace GoodNight.Service.Api.Story
{
  [ApiController]
  [Route("api/v1/stories")]
  public class StoryController : ControllerBase
  {
    [HttpGet()]
    public string Get()
    {
      return "hello?";
    }
  }
}

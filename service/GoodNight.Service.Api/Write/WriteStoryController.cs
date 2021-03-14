using System.Threading.Tasks;
using GoodNight.Service.Domain.Model.Write;
using Microsoft.AspNetCore.Mvc;

namespace GoodNight.Service.Api.Write
{
  [ApiController]
  [Route("api/v1/write/stories")]
  public class WriteStoryController : ControllerBase
  {
    [HttpPost]
    public async Task<ActionResult<Story>> Create()
    {
      System.Console.WriteLine("creating new story.");
      var story = Story.Create("new-story");
      return Ok(story);
    }
  }
}

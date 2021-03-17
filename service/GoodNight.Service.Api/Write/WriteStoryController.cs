using System.Threading.Tasks;
using GoodNight.Service.Domain.Model.Write;
using Microsoft.AspNetCore.Mvc;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Api.Storage;

namespace GoodNight.Service.Api.Write
{
  [ApiController]
  [Route("api/v1/write/stories")]
  public class WriteStoryController : ControllerBase
  {
    private IRepository<Story, string> scenes;

    public WriteStoryController(WriteStore store)
    {
      scenes = store.Stories;
    }

    [HttpPost]
    public ActionResult<Story> Create()
    {
      var storyName = "";

      if (scenes.Get(storyName) is not null)
        return Conflict("A story of the given name already exists.");

      System.Console.WriteLine("creating new story.");
      var story = Story.Create(storyName);
      scenes.Add(story);

      return Ok(story);
    }
  }
}

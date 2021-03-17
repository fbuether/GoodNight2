using System.Threading.Tasks;
using GoodNight.Service.Domain.Model.Write;
using Microsoft.AspNetCore.Mvc;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Api.Storage;
using System.Collections.Generic;

namespace GoodNight.Service.Api.Write
{
  [ApiController]
  [Route("api/v1/write/stories")]
  public class WriteStoryController : ControllerBase
  {
    private IRepository<Story, string> stories;

    public WriteStoryController(WriteStore store)
    {
      stories = store.Stories;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Story>> GetAll()
    {
      return Ok(stories);
    }

    [HttpPost]
    public ActionResult<Story> Create()
    {
      var storyName = "";

      if (stories.Get(storyName) is not null)
        return Conflict("A story of the given name already exists.");

      System.Console.WriteLine("creating new story.");
      var story = Story.Create(storyName);
      stories.Add(story);

      return Ok(story);
    }
  }
}

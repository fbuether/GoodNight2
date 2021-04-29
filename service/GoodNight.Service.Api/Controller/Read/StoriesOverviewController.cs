using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using GoodNight.Service.Domain.Model.Read;
using GoodNight.Service.Storage.Interface;
using System.Linq;

namespace GoodNight.Service.Api.Controller.Read
{
  [ApiController]
  [Route("api/v1/read/stories")]
  public class StoriesOverviewController : ControllerBase
  {
    private IRepository<Story> stories;

    public StoriesOverviewController(IStore store)
    {
      stories = store.Create<Story>();
    }

    [HttpGet]
    public ActionResult<IEnumerable<StoryHeader>> GetAll()
    {
      return Ok(stories.Where(s => s.Public).Select(s => s.ToHeader()));
    }

    [HttpGet("{storyUrlname}")]
    public ActionResult<Story> Get(string storyUrlname)
    {
      var story = stories.Get(storyUrlname);
      if (story is null)
        return NotFound();

      return Ok(story);
    }
  }
}

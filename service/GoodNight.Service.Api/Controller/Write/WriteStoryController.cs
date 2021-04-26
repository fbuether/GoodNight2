using System.Linq;
using System.Threading.Tasks;
using GoodNight.Service.Domain.Model.Write;
using Microsoft.AspNetCore.Mvc;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Api.Storage;
using System.Collections.Generic;
using GoodNight.Service.Domain;

namespace GoodNight.Service.Api.Controller.Write
{
  [ApiController]
  [Route("api/v1/write/stories")]
  public class WriteStoryController : ControllerBase
  {
    private IRepository<Story> stories;

    public WriteStoryController(WriteStore store)
    {
      stories = store.Stories;
    }


    [HttpGet]
    public ActionResult<IEnumerable<StoryHeader>> GetAll()
    {
      // todo: limit to e.g. 100?
      return Ok(stories.Select(s => s.ToHeader()));
    }


    [HttpGet("{urlname}")]
    public ActionResult<StoryHeader> Get(string urlname)
    {
      var story = stories.Get(urlname);
      if (story is null)
        return NotFound();

      return Ok(story.ToHeader());
    }


    [HttpGet("{urlname}/content-by-category")]
    public ActionResult<IEnumerable<Category>> GetContentByCategory(
      string urlname)
    {
      var story = stories.Get(urlname);
      if (story is null)
        return NotFound();

      return Ok(story.GetContentAsCategories());
    }


    public record CreateStoryBody(string name);

    [HttpPost]
    public ActionResult<Story> Create([FromBody] CreateStoryBody newStory)
    {
      var urlname = NameConverter.OfString(newStory.name);

      if (stories.Get(urlname) is not null)
        return Conflict(new ErrorResult(
            "A story of the given name already exists."));

      var story = Story.Create(newStory.name);
      stories.Add(story);

      return Ok(story);
    }
  }
}

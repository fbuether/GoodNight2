using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using GoodNight.Service.Domain;
using GoodNight.Service.Storage.Interface;
using Model = GoodNight.Service.Domain.Model;
using GoodNight.Service.Domain.Model.Write;

namespace GoodNight.Service.Api.Controller.Write
{
  [ApiController]
  [Route("api/v1/write/stories")]
  public class WriteStoryController : ControllerBase
  {
    private IRepository<Story> stories;
    private IRepository<Model.Read.Story> readStories;

    public WriteStoryController(IStore store)
    {
      stories = store.Create<Story>();
      readStories = store.Create<Model.Read.Story>();
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
      readStories.Add(Model.Read.Story.Create(newStory.name));

      return Ok(story);
    }
  }
}

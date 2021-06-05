using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GoodNight.Service.Domain;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Domain.Model.Write;
using Model = GoodNight.Service.Domain.Model;
using GoodNight.Service.Api.Controller.Base;

namespace GoodNight.Service.Api.Controller.Write
{
  [Authorize]
  [ApiController]
  [Route("api/v1/write/stories")]
  public class WriteStoryController : AuthorisedController
  {
    private IRepository<Story> stories;
    private IRepository<Model.Read.Story> readStories;

    public WriteStoryController(IStore store)
      : base(store)
    {
      stories = store.Create<Story>();
      readStories = store.Create<Model.Read.Story>();
    }


    [HttpGet]
    public ActionResult<IEnumerable<StoryHeader>> GetAll()
    {
      return Ok(stories
        .Where(s => s.Creator.Key == GetCurrentUser().Key)
        .Select(s => s.ToHeader()));
    }


    [HttpGet("{urlname}")]
    public ActionResult<StoryHeader> Get(string urlname)
    {
      var story = stories.Get(urlname);
      if (story is null || story.Creator.Key != GetCurrentUser().Key)
        return NotFound();

      return Ok(story.ToHeader());
    }


    [HttpGet("{urlname}/content-by-category")]
    public ActionResult<Category> GetContentByCategory(
      string urlname)
    {
      var story = stories.Get(urlname);
      if (story is null || story.Creator.Key != GetCurrentUser().Key)
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

      var story = Story.Create(newStory.name, GetCurrentUser());
      stories.Add(story);
      readStories.Add(Model.Read.Story.Create(newStory.name));

      return Ok(story);
    }
  }
}

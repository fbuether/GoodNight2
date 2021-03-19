using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using GoodNight.Service.Api.Storage;
using GoodNight.Service.Domain.Model.Write;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Domain.Util;
using Microsoft.AspNetCore.Http;
using System;

namespace GoodNight.Service.Api.Write
{
  [ApiController]
  [Route("api/v1/write/story/{storyUrlname}/scenes")]
  public class WriteSceneController : ControllerBase
  {
    private IRepository<Scene, string> scenes;

    private IRepository<Story, string> stories;

    public WriteSceneController(WriteStore repos)
    {
      scenes = repos.Scenes;
      stories = repos.Stories;
    }


    [HttpGet]
    public ActionResult<IEnumerable<Scene>> GetAll(string storyUrlname)
    {
      var story = stories.FirstOrDefault(s => s.Urlname == storyUrlname);
      if (story is null)
        return NotFound();

      return Ok(story.Scenes);
    }


    public record RawScene(string text);

    [HttpPost]
    public ActionResult<Scene> Create(string storyUrlname,
      [FromBody] RawScene content)
    {
      var story = stories.FirstOrDefault(s => s.Urlname == storyUrlname);
      if (story is null)
        return NotFound();

      var addResult = story.AddNewScene(content.text);


      return addResult.Map<ActionResult<Scene>>(
        scene => {
          scenes.Add(scene);

          return Created(
            $"api/v1/write/story/{storyUrlname}/scene/{scene.Urlname}",
            scene);
        },
        err => this.BadRequest(err));
    }
  }
}

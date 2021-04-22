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
  [Route("api/v1/write/stories/{storyUrlname}/scenes")]
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


    [HttpGet("{sceneUrlname}")]
    public ActionResult<IEnumerable<Scene>> Get(string storyUrlname,
      string sceneUrlname)
    {
      var story = stories.FirstOrDefault(s => s.Urlname == storyUrlname);
      if (story is null)
        return NotFound();

      var scene = story.Scenes.FirstOrDefault(s => s.Urlname == sceneUrlname);
      if (scene is null)
        return NotFound();

      return Ok(scene);
    }


    public record RawScene(string text);

    [HttpPost]
    public ActionResult<Scene> Create(string storyUrlname,
      [FromBody] RawScene content)
    {
      var story = stories.FirstOrDefault(s => s.Urlname == storyUrlname);
      if (story is null)
        return NotFound();

      return story.AddNewScene(content.text)
        .Map<ActionResult<Scene>>(
          (storyScene) => {
            var (story, scene) = storyScene;
            stories.Update(storyUrlname, (_) => story);
            return Created(
              $"api/v1/write/stories/{storyUrlname}/scenes/{scene.Urlname}",
              scene);
          },
          err => BadRequest(new ErrorResult(err)));
    }

    [HttpPut("{sceneUrlname}")]
    public ActionResult<Scene> Update(string storyUrlname, string sceneUrlname,
      [FromBody] RawScene content)
    {
      var story = stories.FirstOrDefault(s => s.Urlname == storyUrlname);
      if (story is null)
        return NotFound();

      var scene = story.Scenes.FirstOrDefault(s => s.Urlname == sceneUrlname);
      if (scene is null)
        return NotFound();

      return story.EditScene(scene, content.text)
        .Map<ActionResult<Scene>>(
          (storyScene) => {
            var (story, scene) = storyScene;
            stories.Update(storyUrlname, (_) => story);
            return Accepted(
              $"api/v1/write/stories/{storyUrlname}/scenes/{scene.Urlname}",
              scene);
          },
          err => BadRequest(new ErrorResult(err)));
    }
  }
}

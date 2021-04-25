using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using GoodNight.Service.Api.Storage;
using GoodNight.Service.Domain.Model.Write;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Domain.Util;

namespace GoodNight.Service.Api.Write
{
  [ApiController]
  [Route("api/v1/write/stories/{storyUrlname}/scenes")]
  public class WriteSceneController : ControllerBase
  {
    private IRepository<Scene> scenes;

    private IRepository<Story> stories;

    public WriteSceneController(WriteStore repos)
    {
      scenes = repos.Scenes;
      stories = repos.Stories;
    }


    [HttpGet]
    public ActionResult<IEnumerable<Scene>> GetAll(string storyUrlname)
    {
      var scenes = stories.FirstOrDefault(s => s.Urlname == storyUrlname)
        ?.Scenes;
      return scenes is not null
        ? Ok(scenes)
        : NotFound();
    }


    [HttpGet("{sceneUrlname}")]
    public ActionResult<IEnumerable<Scene>> Get(string storyUrlname,
      string sceneUrlname)
    {
      var scene = stories.FirstOrDefault(s => s.Urlname == storyUrlname)
        ?.GetScene(sceneUrlname)
        ?.Get();
      return scene is not null
        ? Ok(scene)
        : NotFound();
    }


    public record RawScene(string text);

    [HttpPost]
    public ActionResult<Scene> Create(string storyUrlname,
      [FromBody] RawScene content)
    {
      var story = stories.FirstOrDefault(s => s.Urlname == storyUrlname);
      if (story is null)
        return NotFound();

      return Scene.Parse(content.text)
        .Map(story.SeizeScene)
        .Map(scenes.Add)
        .Bind(s => Result.FailOnNull(s, "The scene already exists."))
        .Map(story.InsertNewScene)
        .Map(ss => ss.MapFirst(stories.Save).Item2)
        .Map<ActionResult<Scene>>(scene => Accepted(
            $"api/v1/write/stories/{storyUrlname}/scenes/{scene.Key}", scene))
        .GetOrError(err => BadRequest(new ErrorResult(err)));
    }

    [HttpPut("{sceneUrlname}")]
    public ActionResult<Scene> Update(string storyUrlname, string sceneUrlname,
      [FromBody] RawScene content)
    {
      var story = stories.FirstOrDefault(s => s.Urlname == storyUrlname);
      if (story is null)
        return NotFound();

      return Scene.Parse(content.text)
        .Filter(s => s.Urlname == sceneUrlname, "Scenes may not change name.")
        .Map(story.SeizeScene)
        .Map(scenes.Update)
        .Bind(s => Result.FailOnNull(s, "The scene does not exist."))
        .Map<ActionResult<Scene>>(scene => Accepted(
            $"api/v1/write/stories/{storyUrlname}/scenes/{scene.Key}", scene))
        .GetOrError(err => BadRequest(new ErrorResult(err)));
    }
  }
}

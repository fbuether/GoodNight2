using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using GoodNight.Service.Domain.Model.Write;
using GoodNight.Service.Domain.Util;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Api.Controller.Write
{
  [ApiController]
  [Route("api/v1/write/stories/{storyUrlname}/scenes")]
  public class WriteSceneController : ControllerBase
  {
    private IRepository<Scene> scenes;

    private IRepository<Story> stories;

    public WriteSceneController(IStore store)
    {
      scenes = store.Create<Scene>();
      stories = store.Create<Story>();
    }


    [HttpGet("{sceneUrlname}")]
    public ActionResult<IEnumerable<Scene>> Get(string storyUrlname,
      string sceneUrlname)
    {
      var scene = stories.FirstOrDefault(s => s.Urlname == storyUrlname)
        ?.GetScene(sceneUrlname);
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
        .Map(story.AddScene)
        .Do(ss => stories.Save(ss.Item1))
        .Map(ss => ss.Item2)
        .Map<ActionResult<Scene>>(scene => Accepted(
            $"api/v1/write/stories/{storyUrlname}/scenes/{scene.Key}",
            scene))
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
        .Map(s => s with {Story = story.Urlname})
        .Map(scenes.Update)
        .Bind(s => Result.FailOnNull(s, "The scene does not exist."))
        .Map<ActionResult<Scene>>(scene => Accepted(
            $"api/v1/write/stories/{storyUrlname}/scenes/{scene.Key}",
            scene.Get()))
        .GetOrError(err => BadRequest(new ErrorResult(err)));
    }
  }
}

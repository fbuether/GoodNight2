using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using GoodNight.Service.Domain.Model.Write;
using GoodNight.Service.Domain.Util;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Domain.Parse;
using Model = GoodNight.Service.Domain.Model;
using GoodNight.Service.Domain;
using System;

namespace GoodNight.Service.Api.Controller.Write
{
  [ApiController]
  [Route("api/v1/write/stories/{storyUrlname}/scenes")]
  public class WriteSceneController : ControllerBase
  {
    private IRepository<Scene> scenes;
    private IRepository<Story> stories;
    private IRepository<Model.Read.Story> readStories;
    private IRepository<Model.Read.Scene> readScenes;
    private IRepository<Model.Read.Quality> readQualities;

    public WriteSceneController(IStore store)
    {
      scenes = store.Create<Scene>();
      stories = store.Create<Story>();
      readStories = store.Create<Model.Read.Story>();
      readScenes = store.Create<Model.Read.Scene>();
      readQualities = store.Create<Model.Read.Quality>();
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

      var parsed = SceneParser.Parse(content.text).ToResult();

      var readStory = readStories.FirstOrDefault(s => s.Key == story.Key);
      if (readStory is null)
        return NotFound();

      var writeScene = parsed
        .Bind(parsed => SceneFactory.Build(parsed, story.Urlname))
        .Map(story.AddScene)
        .Filter(sq => story.Scenes.Any(s => s.Key == sq.Item2.Key),
          "A scene with that name already exists.");

      var readScene = parsed
        .Bind(parsed => Model.Read.SceneFactory.Build(readScenes, readQualities,
            parsed, story.Urlname))
        .Map(readStory.AddScene);

      return writeScene.And(readScene)
        .Do(wsrs => stories.Save(wsrs.Item1.Item1))
        .Do(wsrs => readStories.Save(wsrs.Item2))
        .Map(wr => wr.Item1.Item2)
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

      var parsed = SceneParser.Parse(content.text).ToResult();

      var readStory = readStories.FirstOrDefault(s => s.Key == story.Key);
      if (readStory is null)
        return NotFound();

      var writeScene = parsed
        .Bind(parsed => SceneFactory.Build(parsed, story.Urlname))
        .Map(story.AddScene)
        .Filter(sq => !story.Scenes.Any(s => s.Key == sq.Item2.Key),
          "The scene does not exist.")
        .Filter(sq => sq.Item2.Urlname != sceneUrlname,
          "Scenes may not change their name. Create a new scene.");

      var readScene = parsed
        .Bind(parsed => Model.Read.SceneFactory.Build(readScenes, readQualities,
            parsed, story.Urlname))
        .Map(readStory.AddScene);

      return writeScene.And(readScene)
        .Do(wsrs => stories.Save(wsrs.Item1.Item1))
        .Do(wsrs => readStories.Save(wsrs.Item2))
        .Map(wr => wr.Item1.Item2)
        .Map<ActionResult<Scene>>(scene => Accepted(
            $"api/v1/write/stories/{storyUrlname}/scenes/{scene.Key}",
            scene))
        .GetOrError(err => BadRequest(new ErrorResult(err)));
    }
  }
}

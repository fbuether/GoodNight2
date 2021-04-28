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

    public WriteSceneController(IStore store)
    {
      scenes = store.Create<Scene>();
      stories = store.Create<Story>();
      readStories = store.Create<Model.Read.Story>();
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

      Predicate<string> validateName = (string name) => story.Scenes
        .All(s => s.Get()?.Urlname != NameConverter.OfString(name));

      var parsed = SceneParser.Parse(content.text)
        .ToResult()
        .Filter(parsed => parsed.HasValidName(validateName),
          "A scene with that name already exists.");

      var readStory = readStories.FirstOrDefault(s => s.Key == story.Key);
      if (readStory is null)
        return NotFound();

      var writeScene = parsed
        .Map(parsed => parsed.ToWriteScene())
        .Map(story.AddScene)
        .Do(ss => stories.Save(ss.Item1))
        .Map(ss => ss.Item2);

      var readScene = parsed
        .Map(parsed => parsed.ToReadScene())
        .Map(readStory.AddScene)
        .Do(scene => readStories.Save(scene));

      return writeScene
        .And(readScene)
        .Map(wr => wr.Item1)
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

      Predicate<string> validateName = (string name) => story.Scenes
        .Any(s => s.Get()?.Urlname == NameConverter.OfString(name));

      var parsed = SceneParser.Parse(content.text)
        .ToResult()
        .Filter(parsed => parsed.HasValidName(validateName),
          "A scene with that name does not exist.");

      var readStory = readStories.FirstOrDefault(s => s.Key == story.Key);
      if (readStory is null)
        return NotFound();

      var writeScene = parsed
        .Map(parsed => parsed.ToWriteScene())
        .Map(story.AddScene)
        .Do(ss => stories.Save(ss.Item1))
        .Map(ss => ss.Item2);

      var readScene = parsed
        .Map(parsed => parsed.ToReadScene())
        .Map(readStory.AddScene)
        .Do(scene => readStories.Save(scene));

      return writeScene
        .And(readScene)
        .Map(wr => wr.Item1)
        .Map<ActionResult<Scene>>(scene => Accepted(
            $"api/v1/write/stories/{storyUrlname}/scenes/{scene.Key}",
            scene))
        .GetOrError(err => BadRequest(new ErrorResult(err)));
    }
  }
}

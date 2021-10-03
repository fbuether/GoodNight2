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
using GoodNight.Service.Api.Controller.Base;
using System.Collections.Immutable;

namespace GoodNight.Service.Api.Controller.Write
{
  [ApiController]
  [Route("api/v1/write/stories/{storyUrlname}/scenes")]
  public class WriteSceneController : AuthorisedController
  {
    private IRepository<Scene> scenes;
    private IRepository<Story> stories;
    private IRepository<Quality> qualities;
    private IRepository<Model.Read.Story> readStories;
    private IRepository<Model.Read.Scene> readScenes;
    private IRepository<Model.Read.Quality> readQualities;

    public WriteSceneController(IStore store)
      : base(store)
    {
      scenes = store.Create<Scene>();
      stories = store.Create<Story>();
      qualities = store.Create<Quality>();
      readStories = store.Create<Model.Read.Story>();
      readScenes = store.Create<Model.Read.Scene>();
      readQualities = store.Create<Model.Read.Quality>();
    }


    [HttpGet("{sceneUrlname}")]
    public ActionResult<Scene> Get(string storyUrlname,
      string sceneUrlname)
    {
      var scene = stories
        .FirstOrDefault(s => s.Urlname == storyUrlname &&
          s.Creator.Key == GetCurrentUser().Key)
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
      var story = stories.FirstOrDefault(s => s.Urlname == storyUrlname &&
          s.Creator.Key == GetCurrentUser().Key);
      if (story is null)
        return NotFound();

      var parsed = SceneParser.Parse(content.text).ToResult();

      var readStory = readStories.FirstOrDefault(s => s.Key == story.Key);
      if (readStory is null)
        return NotFound();

      var writeScene = parsed
        .Bind(parsed => SceneFactory.Build(scenes, qualities, readScenes, parsed, story.Urlname))
        .Map(story.AddScene)
        .Assure(sq => scenes.Get(sq.Item2.Key) is null,
          "A scene with that name already exists.");

      var readScene = parsed
        .Bind(parsed => Model.Read.SceneFactory.Build(readScenes, readQualities,
            parsed, story.Urlname))
        .Map(readStory.AddScene);

      return writeScene.And(readScene)
        .Do(wsrs => {
          stories.Save(wsrs.Item1.Item1);
          readStories.Save(wsrs.Item2);

          foreach (var link in LinkHandler.UpdateLinkedScenes(scenes,
              wsrs.Item1.Item2))
          {
            scenes.Save(link);
          }

          foreach (var link in LinkHandler.UpdateLinkedQualities(qualities,
              wsrs.Item1.Item2))
          {
            qualities.Save(link);
          }
        })
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
      var story = stories.FirstOrDefault(s => s.Urlname == storyUrlname &&
        s.Creator.Key == GetCurrentUser().Key);
      if (story is null)
        return NotFound();

      var parsed = SceneParser.Parse(content.text).ToResult();

      var readStory = readStories.FirstOrDefault(s => s.Key == story.Key);
      if (readStory is null)
        return NotFound();

      var writeScene = parsed
        .Bind(parsed => SceneFactory.Build(scenes, qualities, readScenes,
            parsed, story.Urlname))
        .Map(story.AddScene)
        .Assure(sq => scenes.Get(sq.Item2.Key) is not null,
          "The scene does not exist.")
        .Assure(sq => sq.Item2.Urlname == sceneUrlname,
          $"The scene has a different name than before, which is not allowed.");

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

    [HttpDelete("{sceneUrlname}")]
    public ActionResult Delete(string storyUrlname, string sceneUrlname)
    {
      var story = stories.FirstOrDefault(s => s.Urlname == storyUrlname &&
          s.Creator.Key == GetCurrentUser().Key);
      if (story is null)
        return NotFound();

      var key = NameConverter.Concat(storyUrlname, sceneUrlname);
      var scene = scenes.GetReference(key);
      var readScene = readScenes.GetReference(key);

      stories.Update(storyUrlname, story => story.RemoveScene(scene.Key));
      readStories.Update(storyUrlname,
        story => story.RemoveScene(readScene.Key));
      var existed = scenes.Remove(scene.Key);
      existed |= readScenes.Remove(readScene.Key);

      return !existed
        ? NotFound()
        : NoContent();
    }
  }
}

using System;
using System.Collections.Immutable;
using GoodNight.Service.Domain.Model.Parse;
using GoodNight.Service.Domain.Model;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Domain.Parse;
using GoodNight.Service.Domain.Util;

namespace GoodNight.Service.Domain.Model.Write
{
  /// <summary>
  /// A StoryHeader is a Story reduced to its properties required for long
  /// lists of Stories, e.g. Story selection.
  /// </summary>
  public record StoryHeader(
    string name,
    string urlname,
    string description);


  public record Story(
    string Name,
    IImmutableSet<Scene> Scenes,
    IImmutableSet<Quality> Qualities)
    : IStorable<string>
  {
    public string Urlname
    {
      get
      {
        return NameConverter.OfString(Name);
      }
    }

    public string GetKey()
    {
      return Urlname;
    }

    /// <summary>
    /// Create a new story with a given name.
    /// </summary>
    public static Story Create(string name)
    {
      return new Story(name,
        ImmutableHashSet<Scene>.Empty,
        ImmutableHashSet<Quality>.Empty);
    }

    public StoryHeader ToHeader()
    {
      return new StoryHeader(Name, Urlname, "-");
    }


    private Result<Scene, string> Parse(string raw)
    {
      var parser = new SceneParser();
      var parseResult = parser.Parse(raw);
      if (!parseResult.IsSuccessful)
        return new Result.Failure<Scene, string>(parseResult.ErrorMessage!);

      var parsedScene = parseResult.Result!;
      var scene = parsedScene.ToModel();
      return new Result.Success<Scene, string>(scene);
    }

    public Result<(Story, Scene), string> AddNewScene(string raw)
    {
      return Parse(raw).Map(scene => (InsertScene(scene), scene));
    }

    public Result<(Story, Scene), string> EditScene(Scene oldScene, string raw)
    {
      return Parse(raw)
        .Map(scene => (ReplaceScene(oldScene, scene), scene));
    }

    public Story InsertScene(Scene scene)
    {
      return this with { Scenes = this.Scenes.Add(scene) };
    }

    public Story ReplaceScene(Scene oldScene, Scene newScene)
    {
      return this with { Scenes = this.Scenes.Remove(oldScene).Add(newScene) };
    }
  }
}

using System.Collections.Immutable;
using System.Linq;
using GoodNight.Service.Domain.Util;

namespace GoodNight.Service.Domain.Model.Write
{
  public static class SceneFactory
  {
    public static Result<Scene,string> Build(Parse.Scene parsed, string story)
    {
      var contents = parsed.Contents;

      var names = contents.OfType<Parse.Scene.Content.Name>()
        .Select(n => n.DisplayName);
      if (!names.Any())
        return new Result.Failure<Scene,string>(
          "No name given. Add a $name: declaration.");
      var name = names.Last();

      // tags
      var tags = ImmutableArray.CreateRange(
        contents.OfType<Parse.Scene.Content.Tag>()
        .Select(t => t.TagName));

      // category
      var categoryElement = contents.OfType<Parse.Scene.Content.Category>()
        .Select(c => c.Path);
      var category = categoryElement.Any()
        ? categoryElement.Last()
        : ImmutableList<string>.Empty;

      return new Result.Success<Scene, string>(new Scene(
          name, story, parsed.Raw, tags, category));
    }
  }
}

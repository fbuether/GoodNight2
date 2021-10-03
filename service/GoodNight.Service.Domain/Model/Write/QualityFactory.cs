using System.Collections.Immutable;
using System.Linq;
using GoodNight.Service.Domain.Util;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Model.Write
{
  public static class QualityFactory
  {
    private static Result<Quality, string> Fail(string error)
    {
      return new Result.Failure<Quality, string>(error);
    }

    public static Result<Quality,string> Build(
      IRepository<Scene> scenes,
      Parse.Quality parsed, string raw,
      string story)
    {
      var contents = parsed.Contents;

      // name
      var names = contents.OfType<Parse.Quality.Content.Name>()
        .Select(n => n.Value);
      if (!names.Any())
        return Fail("No name given. Add a $name: declaration.");
      var name = names.Last();

      // icon
      var icon = contents.OfType<Parse.Quality.Content.Icon>()
        .Select(i => i.Value)
        .LastOrDefault();

      // tags
      var tags = ImmutableArray.CreateRange(
        contents.OfType<Parse.Quality.Content.Tag>()
        .Select(t => t.Value));

      // category
      var categoryElement = contents.OfType<Parse.Quality.Content.Category>()
        .Select(c => c.Path);
      var category = categoryElement.Any()
        ? categoryElement.First()
        : ImmutableList<string>.Empty;

      // find in-links from stories.
      var inLinks = ImmutableList.CreateRange<IReference<Scene>>(
        scenes.Where(scene => scene.Qualities.Any(q =>
            q.Key == NameConverter.Concat(story, name))));

      return new Result.Success<Quality, string>(new Quality(
          name, story, icon, raw,
          tags, category,
          inLinks));
    }
  }
}

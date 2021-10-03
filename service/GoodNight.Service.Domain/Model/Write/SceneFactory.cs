using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using GoodNight.Service.Domain.Util;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Model.Write
{
  public static class SceneFactory
  {
    public static Result<Scene,string> Build(
      IRepository<Scene> scenes,
      IRepository<Quality> qualities,
      IRepository<Read.Scene> readScenes,
      Parse.Scene parsed, string story)
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
        .SelectMany(t => t.Tags))
        .Sort();

      // category
      var categoryElement = contents.OfType<Parse.Scene.Content.Category>()
        .Select(c => c.Path);
      var category = categoryElement.Any()
        ? categoryElement.Last()
        : ImmutableList<string>.Empty;

      var outLinks = ImmutableList.CreateRange(parsed.GetOutLinks()
        .Select(scene => NameConverter.Concat(story, scene))
        .OrderBy(a => a)
        .Select(scenes.GetReference)
        .Distinct());

      // links from other scenes to this one
      var inLinks = ImmutableList.CreateRange<IReference<Scene>>(
        scenes.Where(scene => scene.OutLinks.Any(s =>
            s.Key == NameConverter.Concat(story, name))));

      // links to qualities used in this scene.
      var qualityRefs = ImmutableList.CreateRange(
        contents.SelectMany(content => CollectQualities(
            (name) => MakeQualityKey(qualities, story, name), content))
        .Distinct());

      // name of the Model.Read.Story.
      var readKey = NameConverter.Concat(story, name);

      return new Result.Success<Scene, string>(new Scene(
          name, story, parsed.Raw,
          readScenes.GetReference(readKey),
          tags, category,
          outLinks, inLinks, qualityRefs));
    }

    private static IReference<Quality> MakeQualityKey(
      IRepository<Quality> qualities, string story, string quality) =>
      qualities.GetReference(NameConverter.Concat(story, quality));

    private static IEnumerable<IReference<Quality>> CollectQualities(
      Func<string, IReference<Quality>> makeRef,
      Parse.Scene.Content content)
    {
      switch (content)
      {
        case Parse.Scene.Content.Set s:
          var quality = makeRef(s.Quality);
          var exprQualities = s.Expression.GetQualities().Select(makeRef);
          return exprQualities.Append(quality);

        case Parse.Scene.Content.Require r:
          return r.Expression.GetQualities().Select(makeRef);

        case Parse.Scene.Content.Option o:
          return o.Content.SelectMany(c => CollectQualities(makeRef, c));

        case Parse.Scene.Content.Condition c:
          return c.If.GetQualities().Select(makeRef)
            .Concat(c.Then.SelectMany(c => CollectQualities(makeRef, c)))
            .Concat(c.Else.SelectMany(c => CollectQualities(makeRef, c)));

        default:
          return ImmutableList<IReference<Quality>>.Empty;
      }
    }
  }
}

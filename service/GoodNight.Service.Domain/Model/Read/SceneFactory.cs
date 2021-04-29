using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using GoodNight.Service.Domain.Util;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Model.Read
{
  public static class SceneFactory
  {
    private static IEnumerable<Scene.Content> ToReadContent(
      IRepository<Scene> scenes,
      IRepository<Quality> qualities,
      Parse.Scene.Content parsed,
      int index)
    {
      switch (parsed)
      {
        case Parse.Scene.Content.Text c:
          yield return new Scene.Content.Text(c.Value);
          break;

        case Parse.Scene.Content.Set c:
          yield return new Scene.Content.Effect(
            qualities.GetReference(c.Quality),
            c.Expression.Map(qualities.GetReference));
          break;

        case Parse.Scene.Content.Option c:
          var description = string.Join("\n", c.Content
            .OfType<Parse.Scene.Content.Text>().Select(t => t.Value));

          var requirements = ImmutableList.CreateRange(
            c.Content.OfType<Parse.Scene.Content.Require>()
            .Select(r => r.Expression.Map(qualities.GetReference)));

          var effects = ImmutableList.CreateRange(
            c.Content.OfType<Parse.Scene.Content.Set>()
            .Select(s => (qualities.GetReference(s.Quality),
                s.Expression.Map(qualities.GetReference))));

          yield return new Scene.Content.Option(
            NameConverter.Concat(c.Scene, index.ToString()),
            description,
            "", // todo: icon
            requirements,
            effects,
            scenes.GetReference(c.Scene));
          break;

        case Parse.Scene.Content.Return c:
          yield return new Scene.Content.Return(scenes.GetReference(c.Scene));
          break;

        case Parse.Scene.Content.Continue c:
          yield return new Scene.Content.Continue(scenes.GetReference(c.Scene));
          break;

        case Parse.Scene.Content.Condition c:
          yield return new Scene.Content.Condition(
            c.If.Map(qualities.GetReference),
            ToReadContentList(scenes, qualities, c.Then),
            ToReadContentList(scenes, qualities, c.Else)
          );
          break;

        case Parse.Scene.Content.Include c:
          yield return new Scene.Content.Include(scenes.GetReference(c.Scene));
          break;
      }
    }

    private static ImmutableList<Scene.Content> ToReadContentList(
      IRepository<Scene> scenes,
      IRepository<Quality> qualities,
      IEnumerable<Parse.Scene.Content> parsedContent)
    {
      return ImmutableList.CreateRange(
        parsedContent.SelectMany((c,i) =>
          ToReadContent(scenes, qualities, c, i)));
    }

    public static Result<Scene,string> Build(
      IRepository<Scene> scenes,
      IRepository<Quality> qualities,
      Parse.Scene parsed, string story)
    {
      var parsedContents = parsed.Contents;

      var names = parsedContents.OfType<Parse.Scene.Content.Name>()
        .Select(n => n.DisplayName);
      if (!names.Any())
        return new Result.Failure<Scene,string>(
          "No name given. Add a $name: declaration.");
      var name = names.Last();

      var isStart = parsedContents.OfType<Parse.Scene.Content.IsStart>().Any();
      var showAlways = parsedContents.OfType<Parse.Scene.Content.ShowAlways>()
        .Any();
      var forceShow = parsedContents.OfType<Parse.Scene.Content.ForceShow>()
        .Any();

      var contents = ToReadContentList(scenes, qualities, parsedContents);

      return new Result.Success<Scene, string>(new Scene(
          name, story, isStart, showAlways, forceShow, contents));
    }
  }
}

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Domain.Util;
using GoodNight.Service.Storage.Interface;
using static GoodNight.Service.Domain.Model.Parse.Scene.Content;

namespace GoodNight.Service.Domain.Model.Read
{
  public static class SceneFactory
  {
    private static Func<string,IReference<Quality>> MakeQuality(
      IRepository<Quality> qualities, string story) =>
      (string qualityUrlname) => qualities.GetReference(
        NameConverter.Concat(story, qualityUrlname));

    private static Func<string,IReference<Scene>> MakeScene(
      IRepository<Scene> scenes, string story) =>
      (string sceneUrlname) => scenes.GetReference(
        NameConverter.Concat(story, sceneUrlname));

    private static IEnumerable<Scene.Content> ToReadContent(
      IRepository<Scene> scenes,
      IRepository<Quality> qualities,
      string story,
      Parse.Scene.Content parsed,
      int index)
    {
      var makeScene = MakeScene(scenes, story);
      var makeQuality = MakeQuality(qualities, story);

      switch (parsed)
      {
        case Parse.Scene.Content.Text c:
          yield return new Scene.Content.Text(c.Value);
          break;

        case Parse.Scene.Content.Set c:
          if (c.Operator == Parse.Scene.Content.SetOperator.Set)
          {
            yield return new Scene.Content.Effect(makeQuality(c.Quality),
                c.Expression.Map(makeQuality));
          }
          else
          {
            Expression.BinaryOperator newOp = c.Operator switch
            {
              SetOperator.Add => new Expression.BinaryOperator.Add(),
              SetOperator.Sub => new Expression.BinaryOperator.Sub(),
              SetOperator.Mult => new Expression.BinaryOperator.Mult(),
              SetOperator.Div => new Expression.BinaryOperator.Div(),
              _ => throw new Exception("Invalid control flow.")
            };

            var quality = makeQuality(c.Quality);
            yield return new Scene.Content.Effect(quality,
              new Expression.BinaryApplication<IReference<Quality>>(
                newOp, new Expression.Quality<IReference<Quality>>(quality),
                c.Expression.Map(makeQuality)));
          }
          break;

        case Parse.Scene.Content.Option c:
          var description = string.Join("\n", c.Content
            .OfType<Parse.Scene.Content.Text>().Select(t => t.Value));

          var requirements = ImmutableList.CreateRange(
            c.Content.OfType<Parse.Scene.Content.Require>()
            .Select(r => r.Expression.Map(makeQuality)));

          var effects = ImmutableList.CreateRange(
            c.Content.OfType<Parse.Scene.Content.Set>()
            .Select(s => (makeQuality(s.Quality),
                s.Expression.Map(makeQuality))));

          yield return new Scene.Content.Option(
            NameConverter.Concat(c.Scene, index.ToString()),
            description,
            "", // todo: icon
            requirements,
            effects,
            makeScene(c.Scene));
          break;

        case Parse.Scene.Content.Return c:
          yield return new Scene.Content.Return(makeScene(c.Scene));
          break;

        case Parse.Scene.Content.Continue c:
          yield return new Scene.Content.Continue(makeScene(c.Scene));
          break;

        case Parse.Scene.Content.Condition c:
          yield return new Scene.Content.Condition(
            c.If.Map(makeQuality),
            ToReadContentList(scenes, qualities, story, c.Then),
            ToReadContentList(scenes, qualities, story, c.Else)
          );
          break;

        case Parse.Scene.Content.Include c:
          yield return new Scene.Content.Include(makeScene(c.Scene));
          break;
      }
    }

    private static ImmutableList<Scene.Content> ToReadContentList(
      IRepository<Scene> scenes,
      IRepository<Quality> qualities,
      string story,
      IEnumerable<Parse.Scene.Content> parsedContent) =>
      ImmutableList.CreateRange(
        parsedContent.SelectMany((c,i) =>
          ToReadContent(scenes, qualities, story, c, i)));

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

      var contents = ToReadContentList(scenes, qualities, story,
        parsedContents);

      return new Result.Success<Scene, string>(new Scene(
          name, story, isStart, showAlways, forceShow, contents));
    }
  }
}

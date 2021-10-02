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

    private static Scene.Content ToReadContent(
      IRepository<Scene> scenes,
      IRepository<Quality> qualities,
      string story,
      Parse.Scene.Content parsed)
    {
      var makeScene = MakeScene(scenes, story);
      var makeQuality = MakeQuality(qualities, story);

      switch (parsed)
      {
        case Parse.Scene.Content.Text c:
          return new Scene.Content.Text(c.Value);

        case Parse.Scene.Content.Set c:
          if (c.Operator == Parse.Scene.Content.SetOperator.Set)
          {
            return new Scene.Content.Effect(makeQuality(c.Quality),
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
            return new Scene.Content.Effect(quality,
              new Expression.BinaryApplication<IReference<Quality>>(
                newOp, new Expression.Quality<IReference<Quality>>(quality),
                c.Expression.Map(makeQuality)));
          }

        case Parse.Scene.Content.Option c:
          var contents = c.Content.Select(c =>
            ToReadContent(scenes, qualities, story, c));
          return new Scene.Content.Option(
            ImmutableList.CreateRange(contents));

        case Parse.Scene.Content.Return c:
          return new Scene.Content.Return(makeScene(c.Scene));

        case Parse.Scene.Content.Continue c:
          return new Scene.Content.Continue(makeScene(c.Scene));

        case Parse.Scene.Content.Condition c:
          return new Scene.Content.Condition(
            c.If.Map(makeQuality),
            ToReadContentList(scenes, qualities, story, c.Then),
            ToReadContentList(scenes, qualities, story, c.Else)
          );

        case Parse.Scene.Content.Include c:
          return new Scene.Content.Include(makeScene(c.Scene));

        default:
          throw new Exception(
            $"Cannot make Read.Content from Parse.Content: {parsed}");
      }
    }

    private static ImmutableList<Scene.Content> ToReadContentList(
      IRepository<Scene> scenes,
      IRepository<Quality> qualities,
      string story,
      IEnumerable<Parse.Scene.Content> parsedContent) =>
      ImmutableList.CreateRange(
        parsedContent.Select(c =>
          ToReadContent(scenes, qualities, story, c)));

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

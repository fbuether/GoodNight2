using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using GoodNight.Service.Domain.Util;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Model.Read
{
  public static class QualityFactory
  {
    private static Result<Quality, string> Fail(string error)
    {
      return new Result.Failure<Quality, string>(error);
    }

    public static Result<Quality,string> Build(
      IRepository<Scene> scenes,
      Parse.Quality parsed,
      string story)
    {
      var contents = parsed.Contents;

      // type
      var types = contents.OfType<Parse.Quality.Content.Type>();
      if (types.Count() != 1)
        return Fail("Not exactly one $type: declaration provided.");
      var type = types.First();

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

      // is hidden
      var isHidden = contents.OfType<Parse.Quality.Content.Hidden>().Any();

      // scene
      var scene = contents.OfType<Parse.Quality.Content.Scene>()
        .Select(s => scenes.GetReference(s.Urlname))
        .LastOrDefault();

      // text
      var text = string.Join("\n", contents.OfType<Parse.Quality.Content.Text>()
        .Select(t => t.Value)).Trim();

      switch (type.Value)
      {
        case Expressions.Type.Bool:
          return new Result.Success<Quality, string>(new Quality.Bool(
              name, story, icon, text, isHidden, scene));

        case Expressions.Type.Int:
          var min = contents.OfType<Parse.Quality.Content.Minimum>()
            .Select(m => m.Value).LastOrDefault();
          var max = contents.OfType<Parse.Quality.Content.Maximum>()
            .Select(m => m.Value).LastOrDefault();

          return new Result.Success<Quality, string>(new Quality.Int(
              name, story, icon, text, isHidden, scene, min, max));

        case Expressions.Type.Enum:
          var levelsEnum = contents.OfType<Parse.Quality.Content.Level>()
            .Select(l => KeyValuePair.Create(l.Number, l.Description));
          var levels = ImmutableDictionary.CreateRange(levelsEnum);

          return new Result.Success<Quality, string>(new Quality.Enum(
              name, story, icon, text, isHidden, scene, levels));
      }

      // cannot happen.
      throw new Exception();
    }
  }
}

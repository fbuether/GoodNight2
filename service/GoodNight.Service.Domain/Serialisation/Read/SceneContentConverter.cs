using System;
using System.Linq;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Domain.Model.Read;
using GoodNight.Service.Storage.Interface;
using SysType = System.Type;
using System.Collections.Generic;

namespace GoodNight.Service.Domain.Serialisation.Read
{
  using QExpr = Expression<IReference<Quality>>;

  public class SceneContentConverter : JsonConverter<Scene.Content>
  {
    private record SerialisedContent(
      string type,
      string? value,
      IReference<Quality>? quality,
      QExpr? expression,
      string? urlname,
      string? description,
      string? icon,
      List<QExpr>? requirements,
      List<Tuple<IReference<Quality>,QExpr>>? effects,
      IReference<Scene>? scene,
      QExpr? ifExpression,
      List<Scene.Content>? thenExpression,
      List<Scene.Content>? elseExpression);

    public override Scene.Content? Read(ref Utf8JsonReader reader,
      SysType typeToConvert, JsonSerializerOptions options)
    {
      var c = JsonSerializer.Deserialize<SerialisedContent>(
        ref reader, options);

      if (c is null)
        throw new JsonException();

      switch (c.type)
      {
        case "text":
          if (c.value is null)
            throw new JsonException();
          return new Scene.Content.Text(c.value);

        case "effect":
          if (c.quality is null || c.expression is null)
            throw new JsonException();
          return new Scene.Content.Effect(c.quality, c.expression);

        case "option":
          if (c.urlname is null || c.scene is null)
            throw new JsonException();
          return new Scene.Content.Option(c.urlname,
            c.description ?? "",
            c.icon,
            c.requirements is not null
            ? ImmutableList.CreateRange(c.requirements)
            : ImmutableList<QExpr>.Empty,
            c.effects is not null
            ? ImmutableList.CreateRange(c.effects
              .Select(e => (e.Item1,e.Item2)))
            : ImmutableList<(IReference<Quality>,QExpr)>.Empty,
            c.scene);

        case "return":
          if (c.scene is null)
            throw new JsonException();
          return new Scene.Content.Return(c.scene);

        case "continue":
          if (c.scene is null)
            throw new JsonException();
          return new Scene.Content.Continue(c.scene);

        case "condition":
          if (c.ifExpression is null)
            throw new JsonException();
          return new Scene.Content.Condition(c.ifExpression,
            c.thenExpression is not null
            ? ImmutableList.CreateRange(c.thenExpression)
            : ImmutableList<Scene.Content>.Empty,
            c.elseExpression is not null
            ? ImmutableList.CreateRange(c.elseExpression)
            : ImmutableList<Scene.Content>.Empty);

        case "include":
          if (c.scene is null)
            throw new JsonException();
          return new Scene.Content.Include(c.scene);

        default:
          return null;
      }
    }

    public override void Write(Utf8JsonWriter writer,
      Scene.Content value, JsonSerializerOptions options)
    {
      switch (value) {
        case Scene.Content.Text c:
          JsonSerializer.Serialize(writer, new SerialisedContent("text",
              c.Value, null, null, null, null, null, null, null, null, null,
              null, null), options);
          break;

        case Scene.Content.Effect c:
          JsonSerializer.Serialize(writer, new SerialisedContent("effect",
              null, c.Quality, c.Expression, null, null, null, null, null, null,
              null, null, null), options);
          break;

        case Scene.Content.Option c:
          var effects = c.Effects.Select(e => Tuple.Create(e.Item1,e.Item2))
            .ToList();
          var option = new SerialisedContent("option",
              null, null, null, c.Urlname, c.Description, c.Icon,
              c.Requirements.ToList(), effects, c.Scene, null, null, null);
          JsonSerializer.Serialize<SerialisedContent>(writer, option, options);
          break;

        case Scene.Content.Return c:
          JsonSerializer.Serialize(writer, new SerialisedContent("return",
              null, null, null, null, null, null, null, null, c.Scene, null,
              null, null), options);
          break;

        case Scene.Content.Continue c:
          JsonSerializer.Serialize(writer, new SerialisedContent("continue",
              null, null, null, null, null, null, null, null, c.Scene, null,
              null, null), options);
          break;

        case Scene.Content.Condition c:
          JsonSerializer.Serialize(writer, new SerialisedContent("condition",
              null, null, null, null, null, null, null, null, null,
              c.If, c.Then.ToList(), c.Else.ToList()), options);
          break;

        case Scene.Content.Include c:
          JsonSerializer.Serialize(writer, new SerialisedContent("include",
              null, null, null, null, null, null, null, null, c.Scene, null,
              null, null), options);
          break;
      }
    }
  }
}

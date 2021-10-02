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
      IReference<Quality>? quality2,
      QExpr? expression,
      List<Scene.Content>? content,
      List<Scene.Content>? content2,
      IReference<Scene>? scene,
      QExpr? ifExpression);

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
          if (c.content is null)
            throw new JsonException();
          return new Scene.Content.Option(ImmutableList.CreateRange(c.content));

        case "requirement":
          if (c.expression is null)
            throw new JsonException();
          return new Scene.Content.Requirement(c.expression);

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
            c.content is not null
            ? ImmutableList.CreateRange(c.content)
            : ImmutableList<Scene.Content>.Empty,
            c.content2 is not null
            ? ImmutableList.CreateRange(c.content2)
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
              c.Value, null, null, null, null, null, null, null),
            options);
          break;

        case Scene.Content.Effect c:
          JsonSerializer.Serialize(writer, new SerialisedContent("effect",
              null, c.Quality, null, c.Expression, null, null, null, null),
            options);
          break;

        case Scene.Content.Option c:
          JsonSerializer.Serialize(writer, new SerialisedContent("effect",
              null, null, null, null, c.Contents.ToList(), null, null, null),
            options);
          break;

        case Scene.Content.Requirement c:
          JsonSerializer.Serialize(writer, new SerialisedContent("effect",
              null, null, null, c.Expression, null, null, null, null),
            options);
          break;

        case Scene.Content.Return c:
          JsonSerializer.Serialize(writer, new SerialisedContent("return",
              null, null, null, null, null, null, c.Scene, null),
            options);
          break;

        case Scene.Content.Continue c:
          JsonSerializer.Serialize(writer, new SerialisedContent("continue",
              null, null, null, null, null, null, c.Scene, null),
            options);
          break;

        case Scene.Content.Condition c:
          JsonSerializer.Serialize(writer, new SerialisedContent("condition",
              null, null, null, null, c.Then.ToList(), c.Else.ToList(), null,
              c.If),
            options);
          break;

        case Scene.Content.Include c:
          JsonSerializer.Serialize(writer, new SerialisedContent("include",
              null, null, null, null, null, null, c.Scene, null),
            options);
          break;
      }
    }
  }
}

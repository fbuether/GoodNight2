using System;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Domain.Model.Read;
using GoodNight.Service.Storage.Interface;
using SysType = System.Type;

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
      ImmutableList<QExpr>? requirements,
      ImmutableList<(IReference<Quality>, QExpr)>?
        effects,
      IReference<Scene>? scene,
      QExpr? ifExpression,
      ImmutableList<Scene.Content>? thenExpression,
      ImmutableList<Scene.Content>? elseExpression);

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
            c.requirements ?? ImmutableList<QExpr>.Empty,
            c.effects ?? ImmutableList<(IReference<Quality>, QExpr)>.Empty,
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
            c.thenExpression ?? ImmutableList<Scene.Content>.Empty,
            c.elseExpression ?? ImmutableList<Scene.Content>.Empty);

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
      writer.WriteStartObject();

      switch (value) {
        case Scene.Content.Text c:
          writer.WriteString("type", "text");
          writer.WriteString("value", c.Value);
          break;

        case Scene.Content.Effect c:
          writer.WriteString("type", "effect");
          writer.WritePropertyName("quality");
          JsonSerializer.Serialize(writer, c.Quality, options);
          writer.WritePropertyName("expression");
          JsonSerializer.Serialize(writer, c.Expression, options);
          break;

        case Scene.Content.Option c:
          writer.WriteString("type", "option");
          writer.WriteString("urlname", c.Urlname);
          writer.WriteString("description", c.Description);
          if (c.Icon is not null)
            writer.WriteString("icon", c.Icon);
          writer.WritePropertyName("requirements");
          JsonSerializer.Serialize(writer, c.Requirements, options);
          writer.WritePropertyName("effects");
          JsonSerializer.Serialize(writer, c.Effects, options);
          writer.WritePropertyName("scene");
          JsonSerializer.Serialize(writer, c.Scene, options);
          break;

        case Scene.Content.Return c:
          writer.WriteString("type", "return");
          writer.WritePropertyName("scene");
          JsonSerializer.Serialize(writer, c.Scene, options);
          break;

        case Scene.Content.Continue c:
          writer.WriteString("type", "continue");
          writer.WritePropertyName("scene");
          JsonSerializer.Serialize(writer, c.Scene, options);
          break;

        case Scene.Content.Condition c:
          writer.WriteString("type", "condition");
          writer.WritePropertyName("if");
          JsonSerializer.Serialize(writer, c.If, options);
          writer.WritePropertyName("then");
          JsonSerializer.Serialize(writer, c.Then, options);
          writer.WritePropertyName("else");
          JsonSerializer.Serialize(writer, c.Else, options);
          break;

        case Scene.Content.Include c:
          writer.WriteString("type", "include");
          writer.WritePropertyName("scene");
          JsonSerializer.Serialize(writer, c.Scene, options);
          break;
      }

      writer.WriteEndObject();
    }
  }
}

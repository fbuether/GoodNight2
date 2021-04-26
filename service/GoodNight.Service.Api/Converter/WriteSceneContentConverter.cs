using System;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Domain.Model.Write;
using SysType = System.Type;

namespace GoodNight.Service.Api.Converter
{
  internal class WriteSceneContentConverter : JsonConverter<Content>
  {
    public override Content? Read(ref Utf8JsonReader reader,
      SysType typeToConvert, JsonSerializerOptions options)
    {
      if (reader.TokenType != JsonTokenType.StartObject)
      {
        throw new JsonException();
      }

      string type = "";
      string value = "";
      Expression<string> expr = new Expression<string>.Bool<string>(false);
      string scene = "";
      ImmutableList<Content> content = ImmutableList<Content>.Empty;
      Expression<string> ifExpr = expr;
      ImmutableList<Content> thenExpr = ImmutableList<Content>.Empty;
      ImmutableList<Content> elseExpr = ImmutableList<Content>.Empty;

      while (reader.Read())
      {
        if (reader.TokenType == JsonTokenType.EndObject)
        {
          switch (type) {
            case "text": return new Content.Text(value);
            case "require": return new Content.Require(expr);
            case "option": return new Content.Option(scene, content);
            case "condition": return new Content.Condition(ifExpr, thenExpr,
              elseExpr);
            case "include": return new Content.Include(scene);
            default: throw new JsonException();
          }
        }
        else if (reader.TokenType == JsonTokenType.PropertyName)
        {
          var property = reader.GetString();
          if (property is not null)
            property = property.ToLower();
          reader.Read();

          if (property == "type")
            type = reader.GetString() ?? type;
          else if (property == "value")
            value = reader.GetString() ?? value;
          else if (property == "expression")
            expr = JsonSerializer.Deserialize<Expression<string>>(
              ref reader, options) ?? expr;
          else if (property == "scene")
            scene = reader.GetString() ?? scene;
          else if (property == "content")
            content = JsonSerializer.Deserialize<ImmutableList<Content>>(
              ref reader, options) ?? content;
          else if (property == "if")
            ifExpr = JsonSerializer.Deserialize<Expression<string>>(
              ref reader, options) ?? ifExpr;
          else if (property == "then")
            thenExpr = JsonSerializer.Deserialize<ImmutableList<Content>>(
              ref reader, options) ?? thenExpr;
          else if (property == "else")
            elseExpr = JsonSerializer.Deserialize<ImmutableList<Content>>(
              ref reader, options) ?? elseExpr;
        }
      }

      throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Content value,
      JsonSerializerOptions options)
    {
      writer.WriteStartObject();

      switch (value) {
        case Content.Text t:
          writer.WriteString("type", "text");
          writer.WritePropertyName("value");
          JsonSerializer.Serialize(writer, t.Value, options);
          break;

        case Content.Require r:
          writer.WriteString("type", "require");
          writer.WritePropertyName("expression");
          JsonSerializer.Serialize(writer, r.Expression, options);
          break;

        case Content.Option o:
          writer.WriteString("type", "option");
          writer.WriteString("scene", o.Scene);
          writer.WritePropertyName("content");
          JsonSerializer.Serialize(writer, o.Contents, options);
          break;

        case Content.Condition c:
          writer.WriteString("type", "condition");
          writer.WritePropertyName("if");
          JsonSerializer.Serialize(writer, c.If, options);
          writer.WritePropertyName("then");
          JsonSerializer.Serialize(writer, c.Then, options);
          writer.WritePropertyName("else");
          JsonSerializer.Serialize(writer, c.Else, options);
          break;

        case Content.Include i:
          writer.WriteString("type", "include");
          writer.WriteString("scene", i.Scene);
          break;
      }

      writer.WriteEndObject();
    }
  }
}

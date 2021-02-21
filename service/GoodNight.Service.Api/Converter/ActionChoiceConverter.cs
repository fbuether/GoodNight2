using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using GoodNight.Service.Domain.Model.Expressions;

namespace GoodNight.Service.Api.Converter
{
  internal class ActionChoiceConverter : JsonConverter<Choice>
  {
    public override Choice? Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
    {
      string type = "";
      string scene = "";
      string text = "";
      ImmutableList<Property> effects =
        ImmutableList<Property>.Empty;

      if (reader.TokenType != JsonTokenType.StartObject)
      {
        throw new JsonException();
      }

      while (reader.Read())
      {
        if (reader.TokenType == JsonTokenType.EndObject)
        {
          switch (type) {
            case "continue": return new Choice.Continue(scene);
            case "return": return new Choice.Return(scene);
            case "option": return new Choice.Action(scene, text, effects);
            default: return null;
          }
        }
        else if (reader.TokenType == JsonTokenType.PropertyName)
        {
          var property = reader.GetString();
          reader.Read();

          if (property == "Continue")
          {
            type = "continue";
            scene = reader.GetString() ?? "";
          }
          else if (property == "Return")
          {
            type = "return";
            scene = reader.GetString() ?? "";
          }
          else if (property == "Option")
          {
            type = "option";
            scene = reader.GetString() ?? "";
          }
          else if (property == "Text")
          {
            text = reader.GetString() ?? "";
          }
          else if (property is not null && property.ToLower() == "effects")
          {
            effects = JsonSerializer
              .Deserialize<ImmutableList<Property>>(
                ref reader, options)
              ?? ImmutableList<Property>.Empty;
          }
        }
      }

      throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Choice value, JsonSerializerOptions options)
    {
      writer.WriteStartObject();

      switch (value)
      {
        case Choice.Action o:
          writer.WriteString("kind", "action");
          writer.WriteString("scene", o.Scene);
          writer.WriteString("text", o.Text);
          writer.WritePropertyName("effects");
          JsonSerializer.Serialize(writer, o.Effects, options);
          break;

        case Choice.Continue c:
          writer.WriteString("kind", "continue");
          writer.WriteString("scene", c.Scene);
          break;

        case Choice.Return c:
          writer.WriteString("kind", "return");
          writer.WriteString("scene", c.Scene);
          break;
      }

      writer.WriteEndObject();
    }
  }
}

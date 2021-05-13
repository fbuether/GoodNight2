using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using GoodNight.Service.Domain.Model.Read;

namespace GoodNight.Service.Api.Converter
{
  public class ActionChoiceConverter : JsonConverter<Choice>
  {
    public override Choice? Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
    {
      string type = "";
      string urlname = "";
      string text = "";
      string? icon = null;
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
            case "continue": return new Choice.Continue();
            case "return": return new Choice.Return();
            case "option": return new Choice.Action(
              urlname, text, icon, effects);
            default: return null;
          }
        }
        else if (reader.TokenType == JsonTokenType.PropertyName)
        {
          var property = reader.GetString();
          reader.Read();

          if (property == "kind")
          {
            type = reader.GetString() ?? "";
          }
          else if (property == "urlname")
          {
            urlname = reader.GetString() ?? "";
          }
          else if (property == "text")
          {
            text = reader.GetString() ?? "";
          }
          else if (property == "icon")
          {
            icon = reader.GetString() ?? "";
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
          writer.WriteString("text", o.Text);
          writer.WriteString("text", o.Urlname);
          writer.WriteString("text", o.Icon);
          writer.WritePropertyName("effects");
          JsonSerializer.Serialize(writer, o.Effects, options);
          break;

        case Choice.Continue c:
          writer.WriteString("kind", "continue");
          break;

        case Choice.Return c:
          writer.WriteString("kind", "return");
          break;
      }

      writer.WriteEndObject();
    }
  }
}

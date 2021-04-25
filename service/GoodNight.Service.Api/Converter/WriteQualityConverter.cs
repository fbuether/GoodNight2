using System;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using GoodNight.Service.Domain.Model.Write;
using ExpType = GoodNight.Service.Domain.Model.Expressions.Type;

namespace GoodNight.Service.Api.Converter
{
  internal class WriteQualityConverter : JsonConverter<Quality>
  {
    public override Quality? Read(ref Utf8JsonReader reader, Type typeToConvert,
      JsonSerializerOptions options)
    {
      if (reader.TokenType != JsonTokenType.StartObject)
      {
        throw new JsonException();
      }

      string type = "";
      string name = "";
      string story = "";
      string raw = "";
      bool hidden = false;
      ImmutableList<string> tags = ImmutableList<string>.Empty;
      ImmutableList<string> category = ImmutableList<string>.Empty;
      string? scene = null;
      string description = "";
      int? min = null;
      int? max = null;
      ImmutableDictionary<int, string> levels =
        ImmutableDictionary<int, string>.Empty;

      while (reader.Read())
      {
        if (reader.TokenType == JsonTokenType.EndObject)
        {
          switch (type) {
            case "bool": return new Quality.Bool(name, story, raw, hidden, tags,
              category, scene, description);
            case "int": return new Quality.Int(name, story, raw, hidden, tags,
              category, scene, description, min, max);
            case "enum": return new Quality.Enum(name, story, raw, hidden, tags,
              category, scene, description, levels);
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
          else if (property == "name")
            name = reader.GetString() ?? name;
          else if (property == "story")
            story = reader.GetString() ?? story;
          else if (property == "raw")
            raw = reader.GetString() ?? raw;
          else if (property == "hidden")
            hidden = reader.GetBoolean();
          else if (property == "tags")
            tags = JsonSerializer.Deserialize<ImmutableList<string>>(
              ref reader, options) ?? tags;
          else if (property == "category")
            category = JsonSerializer.Deserialize<ImmutableList<string>>(
              ref reader, options) ?? category;
          else if (property == "scene")
            scene = reader.GetString();
          else if (property == "description")
            description = reader.GetString() ?? description;
          else if (property == "minimum")
            min = reader.GetInt32();
          else if (property == "maximum")
            max = reader.GetInt32();
          else if (property == "levels")
            levels = JsonSerializer
              .Deserialize<ImmutableDictionary<int, string>>(ref reader,
                options) ?? levels;
        }
      }

      throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Quality value,
      JsonSerializerOptions options)
    {
      writer.WriteStartObject();

      writer.WritePropertyName("category");
      JsonSerializer.Serialize(writer, value.Category, options);
      writer.WriteString("description", value.Description);
      writer.WriteBoolean("hidden", value.Hidden);
      writer.WriteString("name", value.Name);
      writer.WriteString("raw", value.Raw);
      if (value.Scene is not null)
        writer.WriteString("scene", value.Scene);
      writer.WriteString("story", value.Story);
      writer.WritePropertyName("tags");
      JsonSerializer.Serialize(writer, value.Tags, options);
      writer.WriteString("urlname", value.Urlname);


      switch (value)
      {
        case Quality.Bool b:
          writer.WriteString("type", "bool");
          break;
        case Quality.Int i:
          writer.WriteString("type", "int");
          if (i.Minimum is not null)
            writer.WriteNumber("minimum", i.Minimum.Value);
          if (i.Maximum is not null)
            writer.WriteNumber("minimum", i.Maximum.Value);
          break;
        case Quality.Enum e:
          writer.WriteString("type", "enum");
          writer.WritePropertyName("levels");
          JsonSerializer.Serialize(writer, e.Levels, options);
          break;
      }

      writer.WriteEndObject();
    }
  }
}

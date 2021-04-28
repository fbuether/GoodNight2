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

      string name = "";
      string story = "";
      string? icon = null;
      string raw = "";
      ImmutableList<string> tags = ImmutableList<string>.Empty;
      ImmutableList<string> category = ImmutableList<string>.Empty;

      while (reader.Read())
      {
        if (reader.TokenType == JsonTokenType.EndObject)
        {
          return new Quality(name, story, icon, raw, tags, category);
        }
        else if (reader.TokenType == JsonTokenType.PropertyName)
        {
          var property = reader.GetString();
          if (property is not null)
            property = property.ToLower();
          reader.Read();

          if (property == "name")
            name = reader.GetString() ?? name;
          else if (property == "story")
            story = reader.GetString() ?? story;
          else if (property == "icon")
            icon = reader.GetString() ?? icon;
          else if (property == "raw")
            raw = reader.GetString() ?? raw;
          else if (property == "tags")
            tags = JsonSerializer.Deserialize<ImmutableList<string>>(
              ref reader, options) ?? tags;
          else if (property == "category")
            category = JsonSerializer.Deserialize<ImmutableList<string>>(
              ref reader, options) ?? category;
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
      if (value.Icon is not null)
        writer.WriteString("icon", value.Icon);
      writer.WriteString("name", value.Name);
      writer.WriteString("raw", value.Raw);
      writer.WriteString("story", value.Story);
      writer.WritePropertyName("tags");
      JsonSerializer.Serialize(writer, value.Tags, options);
      writer.WriteString("urlname", value.Urlname);

      writer.WriteEndObject();
    }
  }
}

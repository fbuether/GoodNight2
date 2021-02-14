using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Domain.Read;

namespace GoodNight.Service.Api.Converter
{
  internal class ExpressionValueConverter : JsonConverter<Value>
  {
    public override Value? Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
    {
      Value? val = null;

      if (reader.TokenType != JsonTokenType.StartObject)
      {
        throw new JsonException();
      }

      reader.Read();
      if (reader.TokenType != JsonTokenType.PropertyName)
      {
        throw new JsonException();
      }

      var property = reader.GetString();
      reader.Read();
      if (property == "Bool")
      {
        val = new Value.Bool(reader.GetBoolean());
      }
      else if (property == "Int")
      {
        val = new Value.Int(reader.GetInt32());
      }
      else if (property == "Enum")
      {
        val = new Value.Enum(reader.GetInt32());
      }

      while (reader.TokenType != JsonTokenType.EndObject)
      {
        reader.Read();
      }

      return val;
    }

    public override void Write(Utf8JsonWriter writer, Value value, JsonSerializerOptions options)
    {
      writer.WriteStartObject();

      switch (value)
      {
        case Value.Bool b:
          writer.WriteString("kind", "Bool");
          writer.WriteBoolean("value", b.Value);
          break;

        // must come before value.Int so it's not used up there.
        case Value.Enum e:
          writer.WriteString("kind", "Enum");
          writer.WriteNumber("value", e.Value);
          break;

        case Value.Int i:
          writer.WriteString("kind", "Int");
          writer.WriteNumber("value", i.Value);
          break;
      }

      writer.WriteEndObject();
    }
  }
}

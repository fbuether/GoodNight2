using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using GoodNight.Service.Domain.Model.Expressions;

namespace GoodNight.Service.Domain.Serialisation.Expressions
{
  public class ExpressionValueConverter : JsonConverter<Value>
  {
    private record SerialisedValue(string type, bool? bValue, int? iValue);

    public override Value? Read(ref Utf8JsonReader reader,
      System.Type typeToConvert, JsonSerializerOptions options)
    {
      var serialised = JsonSerializer.Deserialize<SerialisedValue>(ref reader,
        options);
      if (serialised is null)
        throw new JsonException();

      switch (serialised.type)
      {
        case "bool":
          if (serialised.bValue is null)
            throw new JsonException();
          return new Value.Bool(serialised.bValue.Value);
        case "int":
        case "enum":
          if (serialised.iValue is null)
            throw new JsonException();
          return new Value.Int(serialised.iValue.Value);
        default:
          throw new JsonException();
      }
    }

    public override void Write(Utf8JsonWriter writer, Value value,
      JsonSerializerOptions options)
    {
      switch (value)
      {
        case Value.Bool v:
          JsonSerializer.Serialize(writer, new SerialisedValue("bool", v.Value,
              null), options);
          break;

        case Value.Int v:
          JsonSerializer.Serialize(writer, new SerialisedValue("int", null,
              v.Value), options);
          break;
      }
    }
  }
}

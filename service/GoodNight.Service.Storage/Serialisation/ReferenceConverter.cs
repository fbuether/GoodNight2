using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Storage.Serialisation
{
  public class ReferenceConverter<T> : JsonConverter<IReference<T>>
    where T : class, IStorable
  {
    private Store store;

    public ReferenceConverter(Store store)
    {
      this.store = store;
    }

    private static void ReadStartObject(ref Utf8JsonReader reader)
    {
      reader.Read();
      if (reader.TokenType != JsonTokenType.StartObject)
        throw new JsonException($"Expected StartObject, but got {reader.TokenType}.");
    }

    private static void ReadFinishObject(ref Utf8JsonReader reader)
    {
      reader.Read();
      if (reader.TokenType != JsonTokenType.EndObject)
        throw new JsonException($"Expected StartObject, but got {reader.TokenType}.");
    }

    private static string ReadPropertyName(ref Utf8JsonReader reader,
      string? expected = null)
    {
      reader.Read();
      if (reader.TokenType != JsonTokenType.PropertyName)
        throw new JsonException($"Expected PropertyName, but got {reader.TokenType}.");

      var name = reader.GetString();
      if (name is null)
        throw new JsonException($"At PropertyName read null name.");

      if (expected is not null)
      {
        if (name != expected)
          throw new JsonException($"Expected PropertyName \"{expected}\", but got \"{name}\".");
      }

      return name;
    }

    private static string ReadStringProperty(ref Utf8JsonReader reader)
    {
      reader.Read();
      if (reader.TokenType != JsonTokenType.String)
        throw new JsonException($"Expected String, but got {reader.TokenType}.");

      var value = reader.GetString();
      if (value is null)
        throw new JsonException($"At String read null value.");

      return value;
    }


    public override IReference<T>? Read(ref Utf8JsonReader reader,
      Type typeToConvert, JsonSerializerOptions options)
    {
      // for some reason the object start is already read.
      // ReadStartObject(ref reader);
      ReadPropertyName(ref reader, "repository");
      var repository = ReadStringProperty(ref reader);
      ReadPropertyName(ref reader, "key");
      var key = ReadStringProperty(ref reader);
      ReadFinishObject(ref reader);

      return store.GetRepository<T>(repository)?.GetReference(key);
    }

    public override void Write(Utf8JsonWriter writer, IReference<T> value,
      JsonSerializerOptions options)
    {
      var refValue = value as Reference<T>;
      if (refValue is null)
        throw new Exception();

      writer.WriteStartObject();
      writer.WriteString("repository", refValue.Repository.UniqueName);
      writer.WriteString("key", refValue.Key);
      writer.WriteEndObject();
    }
  }
}

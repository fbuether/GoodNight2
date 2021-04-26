using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace GoodNight.Service.Storage.Journal
{
  public static class JournalReader
  {
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

    private static object ReadObject(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
    {
      if (type == typeof(string))
      {
        return ReadStringProperty(ref reader);
      }

      var obj = JsonSerializer.Deserialize(ref reader, type, options);
      if (obj is null)
        throw new JsonException($"Reading \"{nameof(type)}\", but Deserialize returned null.");

      return obj;
    }

    private static long ReadEntry(byte[] bytes, Store store, JsonSerializerOptions options)
    {
      var reader = new Utf8JsonReader(bytes);

      ReadStartObject(ref reader);

      ReadPropertyName(ref reader, "repos");
      var reposName = ReadStringProperty(ref reader);

      var repos = store.GetRepository(reposName);
      if (repos is null)
        throw new JsonException($"Got Entry for repos \"{reposName}\", but repos does not exist.");

      ReadPropertyName(ref reader, "kind");
      var kind = ReadStringProperty(ref reader);

      switch (kind)
      {
        case "Add":
          ReadPropertyName(ref reader, "value");
          var addValue = ReadObject(ref reader, repos.ValueType, options);
          var added = repos.Replay(new Entry.Add(reposName, addValue));
          if (!added)
            throw new JsonException($"Did not replay Entry.Add({reposName}, {addValue})");
          break;

        case "Update":
          ReadPropertyName(ref reader, "key");
          var updKey = ReadStringProperty(ref reader);
          ReadPropertyName(ref reader, "value");
          var updValue = ReadObject(ref reader, repos.ValueType, options);
          var updated = repos.Replay(new Entry.Update(reposName, updKey, updValue));
          if (!updated)
            throw new JsonException($"Did not replay Entry.Update({reposName}, {updKey}, {updValue})");
          break;

        case "Delete":
          ReadPropertyName(ref reader, "key");
          var delKey = ReadStringProperty(ref reader);
          var deleted = repos.Replay(new Entry.Delete(reposName, delKey));
          if (!deleted)
            throw new JsonException($"Did not replay Entry.Delete({reposName}, {delKey})");
          break;
      };

      ReadFinishObject(ref reader);

      return reader.BytesConsumed;
    }

    public static void ReadAll(Stream stream, Store store, JsonSerializerOptions options)
    {
      stream.Seek(0, SeekOrigin.Begin);

      var reader = new StreamReader(stream, Encoding.UTF8);
      while (!reader.EndOfStream)
      {
        var line = reader.ReadLine();
        if (line is null)
          break;

        var bytes = Encoding.UTF8.GetBytes(line);
        var readCount = ReadEntry(bytes, store, options);

        if (readCount < bytes.Length)
        {
          var rem = Encoding.UTF8.GetString(bytes.AsSpan((int)readCount));
          Console.WriteLine($"-- ignoring remaining string: {rem} ({bytes.Length - readCount})");
        }
      }
    }
  }
}

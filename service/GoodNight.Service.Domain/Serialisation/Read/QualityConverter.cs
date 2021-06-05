using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using GoodNight.Service.Domain.Model.Read;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Serialisation.Read
{
  public class QualityConverter : JsonConverter<Quality>
  {
    private record SerialisedQuality(string Kind,
      string Name, string Story, string? Icon, string Description, bool Hidden,
      IReference<Scene>? Scene, int? Minimum, int? Maximum,
      Dictionary<int, string>? Values);

    public override Quality? Read(ref Utf8JsonReader reader, Type typeToConvert,
      JsonSerializerOptions options)
    {
      var serialised = JsonSerializer.Deserialize<SerialisedQuality>(ref reader,
        options);
      if (serialised is null)
        throw new JsonException();

      switch (serialised.Kind)
      {
        case "Bool":
          return new Quality.Bool(serialised.Name, serialised.Story,
            serialised.Icon, serialised.Description, serialised.Hidden,
            serialised.Scene);
        case "Int":
          return new Quality.Int(serialised.Name, serialised.Story,
            serialised.Icon, serialised.Description, serialised.Hidden,
            serialised.Scene, serialised.Minimum, serialised.Maximum);
        case "Enum":
          if (serialised.Values is null)
            throw new JsonException();

          return new Quality.Enum(serialised.Name, serialised.Story,
            serialised.Icon, serialised.Description, serialised.Hidden,
            serialised.Scene,
            ImmutableDictionary.CreateRange(serialised.Values));
        default:
          throw new JsonException();
      }
    }

    public override void Write(Utf8JsonWriter writer, Quality value,
      JsonSerializerOptions options)
    {
      switch (value)
      {
        case Quality.Bool q:
          JsonSerializer.Serialize(writer, new SerialisedQuality("Bool",
              q.Name, q.Story, q.Icon, q.Description, q.Hidden, q.Scene,
              null, null, null),
            options);
          break;

        case Quality.Int q:
          JsonSerializer.Serialize(writer, new SerialisedQuality("Int",
              q.Name, q.Story, q.Icon, q.Description, q.Hidden, q.Scene,
              q.Minimum, q.Maximum, null),
            options);
          break;

        case Quality.Enum q:
          JsonSerializer.Serialize(writer, new SerialisedQuality("Enum",
              q.Name, q.Story, q.Icon, q.Description, q.Hidden, q.Scene,
              null, null, new Dictionary<int,string>(q.Values)),
            options);
          break;
      }
    }
  }
}

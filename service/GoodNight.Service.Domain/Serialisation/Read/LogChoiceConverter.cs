using System;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using GoodNight.Service.Domain.Model.Read;

namespace GoodNight.Service.Domain.Serialisation.Read
{
  public class LogChoiceConverter : JsonConverter<Choice>
  {
    public record SerialisedChoice(
      string Kind,

      string? Urlname,
      string? Text,
      IImmutableList<Property>? Effects);

    public override Choice? Read(ref Utf8JsonReader reader, Type typeToConvert,
      JsonSerializerOptions options)
    {
      var serialised = JsonSerializer.Deserialize<SerialisedChoice>(ref reader,
        options);
      if (serialised is null)
        throw new JsonException();

      switch (serialised.Kind)
      {
        case "Action":
          if (serialised.Urlname is null
            || serialised.Text is null
            || serialised.Effects is null)
            throw new JsonException();

          return new Choice.Action(serialised.Urlname, serialised.Text,
            serialised.Effects);
        case "Return":
          return new Choice.Return();
        case "Continue":
          return new Choice.Continue();
        default:
          throw new JsonException();
      }
    }

    public override void Write(Utf8JsonWriter writer, Choice value,
      JsonSerializerOptions options)
    {
      switch (value)
      {
        case Choice.Action c:
          JsonSerializer.Serialize(writer, new SerialisedChoice("Action",
              c.Urlname, c.Text, c.Effects), options);
          break;
        case Choice.Return c:
          JsonSerializer.Serialize(writer, new SerialisedChoice("Return",
              null, null, null), options);
          break;
        case Choice.Continue c:
          JsonSerializer.Serialize(writer, new SerialisedChoice("Continue",
              null, null, null), options);
          break;
      }
    }
  }
}

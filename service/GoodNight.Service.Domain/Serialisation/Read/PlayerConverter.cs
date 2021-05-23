using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Domain.Model.Read;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Serialisation.Read
{
  public class PlayerConverter : JsonConverter<Player>
  {
    private record SerialisedPlayer(string name,
      List<Tuple<IReference<Quality>, Value>> state);

    public override Player? Read(ref Utf8JsonReader reader,
      System.Type typeToConvert, JsonSerializerOptions options)
    {
      var c = JsonSerializer.Deserialize<SerialisedPlayer>(
        ref reader, options);
      if (c is null)
        throw new JsonException();

      return new Player(c.name, ImmutableList.CreateRange(
          c.state.Select(c => (c.Item1, c.Item2))));
    }

    public override void Write(Utf8JsonWriter writer, Player value,
      JsonSerializerOptions options)
    {
      var serialisedPlayer = new SerialisedPlayer(value.Name,
        value.State.Select(s => Tuple.Create(s.Item1, s.Item2)).ToList());

      JsonSerializer.Serialize(writer, serialisedPlayer, options);
    }
  }
}

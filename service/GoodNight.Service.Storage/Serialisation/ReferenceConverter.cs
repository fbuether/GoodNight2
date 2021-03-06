using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Storage.Serialisation
{
  internal class ReferenceConverter<T> : JsonConverter<IReference<T>>
    where T : class, IStorable<T>
  {
    private record SerialisedReference(string typeName, string key);

    private Store store;

    public ReferenceConverter(Store store)
    {
      this.store = store;
    }

    public override IReference<T>? Read(ref Utf8JsonReader reader,
      Type typeToConvert, JsonSerializerOptions options)
    {
      var serialised = JsonSerializer.Deserialize<SerialisedReference>(
        ref reader, options);
      if (serialised is null)
        throw new JsonException();

      var typeName = typeof(T).AssemblyQualifiedName;
      if (typeName is null)
        throw new Exception();

      if (typeName != serialised.typeName)
        throw new JsonException($"Serialized reference for {typeName}, "
          + $"but read {serialised.typeName} ({serialised.key}).");

      return new Reference<T>(
        store.GetRepository<T>(typeName),
        serialised.key);
    }

    public override void Write(Utf8JsonWriter writer, IReference<T> value,
      JsonSerializerOptions options)
    {
      var typeName = typeof(T).AssemblyQualifiedName;
      if (typeName is null)
        throw new Exception();

      // if this is not a reference, but a live object, add it to the
      // repository, so it can be saved.
      if (value is T tValue)
      {
        store.GetRepository<T>(typeName).Save(tValue);
      }

      var serialised = new SerialisedReference(typeName, value.Key);
      JsonSerializer.Serialize(writer, serialised, options);
    }
  }
}

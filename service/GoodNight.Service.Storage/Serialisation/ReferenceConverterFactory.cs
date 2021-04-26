using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Storage.Serialisation
{
  internal class ReferenceConverterFactory : JsonConverterFactory
  {
    private Store store;

    internal ReferenceConverterFactory(Store store)
    {
      this.store = store;
    }

    public override bool CanConvert(Type typeToConvert)
    {
      return typeToConvert.IsGenericType
        && typeToConvert.GetGenericTypeDefinition() == typeof(IReference<>);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert,
      JsonSerializerOptions options)
    {
      var elementType = typeToConvert.GetGenericArguments().First();

      var repos = Activator.CreateInstance(
        typeof(ReferenceConverter<>).MakeGenericType(elementType),
        new [] { store })
        as JsonConverter;
      if (repos is null)
        throw new Exception(
          $"Could not create ReferenceConverter for {elementType.FullName}");

      return repos;
    }
  }
}

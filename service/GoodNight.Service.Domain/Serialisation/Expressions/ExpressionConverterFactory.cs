using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using GoodNight.Service.Domain.Model.Expressions;

namespace GoodNight.Service.Domain.Serialisation.Expressions
{
  public class ExpressionConverterFactory : JsonConverterFactory
  {
    public override bool CanConvert(System.Type typeToConvert)
    {
      return typeToConvert.IsGenericType
        && typeToConvert.GetGenericTypeDefinition() == typeof(Expression<>);
    }

    public override JsonConverter? CreateConverter(System.Type typeToConvert,
      JsonSerializerOptions options)
    {
      var elementType = typeToConvert.GetGenericArguments().First();

      var converter = Activator.CreateInstance(
        typeof(ExpressionConverter<>).MakeGenericType(elementType))
        as JsonConverter;
      if (converter is null)
        throw new Exception(
          $"Could not create ReferenceConverter for {elementType.FullName}");

      return converter;
    }
  }
}

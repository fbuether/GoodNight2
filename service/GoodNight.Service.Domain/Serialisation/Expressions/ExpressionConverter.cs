using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using GoodNight.Service.Domain.Model.Expressions;

namespace GoodNight.Service.Domain.Serialisation.Expressions
{
  public class ExpressionConverter<T> : JsonConverter<Expression<T>>
    where T : class
  {
    private record SerialisedExpression<TE>(string type,
      TE? Quality,
      bool? Bool,
      int? Number,
      string? UnaryOperator,
      string? BinaryOperator,
      Expression<T>? ArgumentOne,
      Expression<T>? ArgumentTwo);

    public override Expression<T>? Read(ref Utf8JsonReader reader,
      System.Type typeToConvert, JsonSerializerOptions options)
    {
      var c = JsonSerializer.Deserialize<SerialisedExpression<T>>(
        ref reader, options);

      if (c is null)
        throw new JsonException();

      switch (c.type)
      {
        case "quality":
          if (c.Quality is null)
            throw new JsonException();
          return new Expression<T>.Quality<T>(c.Quality);

        case "bool":
          if (c.Bool is null)
            throw new JsonException();
          return new Expression<T>.Bool<T>(c.Bool.Value);

        case "number":
          if (c.Number is null)
            throw new JsonException();
          return new Expression<T>.Number<T>(c.Number.Value);

        case "unary":
          if (c.UnaryOperator is null || c.ArgumentOne is null)
            throw new JsonException();

          if (c.UnaryOperator != "not")
            throw new JsonException();

          return new Expression<T>.UnaryApplication<T>(
            new Expression<T>.UnaryOperator.Not(),
            c.ArgumentOne);

        case "binary":
          if (c.BinaryOperator is null || c.ArgumentOne is null
            || c.ArgumentTwo is null)
            throw new JsonException();

          var op = StringToBinaryOperator(c.BinaryOperator);

          return new Expression<T>.BinaryApplication<T>(
            op, c.ArgumentOne, c.ArgumentTwo);

        default:
          throw new JsonException();
      }
    }

    public override void Write(Utf8JsonWriter writer, Expression<T> value,
      JsonSerializerOptions options)
    {
      switch (value)
      {
        case Expression<T>.Quality<T> e:
          JsonSerializer.Serialize(writer, new SerialisedExpression<T>(
              "quality", e.Value, null, null, null, null, null, null),
            options);
          return;

        case Expression<T>.Bool<T> e:
          JsonSerializer.Serialize(writer, new SerialisedExpression<T>(
              "bool", null, e.Value, null, null, null, null, null),
            options);
          return;

        case Expression<T>.Number<T> e:
          JsonSerializer.Serialize(writer, new SerialisedExpression<T>(
              "number", null, null, e.Value,  null, null, null, null),
            options);
          return;

        case Expression<T>.UnaryApplication<T> e:
          if (e.Operator is not Expression<T>.UnaryOperator.Not)
            throw new JsonException();

          JsonSerializer.Serialize(writer, new SerialisedExpression<T>(
              "unary", null, null, null, "not", null, e.Argument, null),
            options);
          return;

        case Expression<T>.BinaryApplication<T> e:
          var op = BinaryOperatorToString(e.Operator);

          JsonSerializer.Serialize(writer, new SerialisedExpression<T>(
              "binary", null, null, null, null, op, e.Left, e.Right),
            options);
          return;
      }
    }

    private Expression<T>.BinaryOperator StringToBinaryOperator(
      string name)
    {
      if (name == "add") return new Expression<T>.BinaryOperator.Add();
      if (name == "sub") return new Expression<T>.BinaryOperator.Sub();
      if (name == "mult") return new Expression<T>.BinaryOperator.Mult();
      if (name == "div") return new Expression<T>.BinaryOperator.Div();

      if (name == "and") return new Expression<T>.BinaryOperator.And();
      if (name == "or") return new Expression<T>.BinaryOperator.Or();

      if (name == ">") return new Expression<T>.BinaryOperator.Greater();
      if (name == ">=")
        return new Expression<T>.BinaryOperator.GreaterOrEqual();
      if (name == "<") return new Expression<T>.BinaryOperator.Less();
      if (name == "<=") return new Expression<T>.BinaryOperator.LessOrEqual();
      if (name == "=") return new Expression<T>.BinaryOperator.Equal();
      if (name == "!=") return new Expression<T>.BinaryOperator.NotEqual();

      throw new JsonException();
    }

    private string BinaryOperatorToString(Expression<T>.BinaryOperator op)
    {
      switch (op)
      {
        case Expression<T>.BinaryOperator.Add: return "add";
        case Expression<T>.BinaryOperator.Sub: return "sub";
        case Expression<T>.BinaryOperator.Mult: return "mult";
        case Expression<T>.BinaryOperator.Div: return "div";

        case Expression<T>.BinaryOperator.Greater: return ">";
        case Expression<T>.BinaryOperator.GreaterOrEqual: return ">=";
        case Expression<T>.BinaryOperator.Less: return "<";
        case Expression<T>.BinaryOperator.LessOrEqual: return "<=";
        case Expression<T>.BinaryOperator.Equal: return "=";
        case Expression<T>.BinaryOperator.NotEqual: return "!=";
        default:
          throw new JsonException();
      }
    }
    }
}

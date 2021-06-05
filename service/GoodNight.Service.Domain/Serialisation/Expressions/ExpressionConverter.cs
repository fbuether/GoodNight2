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
          return new Expression.Quality<T>(c.Quality);

        case "bool":
          if (c.Bool is null)
            throw new JsonException();
          return new Expression.Bool<T>(c.Bool.Value);

        case "number":
          if (c.Number is null)
            throw new JsonException();
          return new Expression.Number<T>(c.Number.Value);

        case "unary":
          if (c.UnaryOperator is null || c.ArgumentOne is null)
            throw new JsonException();

          if (c.UnaryOperator != "not")
            throw new JsonException();

          return new Expression.UnaryApplication<T>(
            new Expression.UnaryOperator.Not(),
            c.ArgumentOne);

        case "binary":
          if (c.BinaryOperator is null || c.ArgumentOne is null
            || c.ArgumentTwo is null)
            throw new JsonException();

          var op = StringToBinaryOperator(c.BinaryOperator);

          return new Expression.BinaryApplication<T>(
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
        case Expression.Quality<T> e:
          JsonSerializer.Serialize(writer, new SerialisedExpression<T>(
              "quality", e.Value, null, null, null, null, null, null),
            options);
          return;

        case Expression.Bool<T> e:
          JsonSerializer.Serialize(writer, new SerialisedExpression<T>(
              "bool", null, e.Value, null, null, null, null, null),
            options);
          return;

        case Expression.Number<T> e:
          JsonSerializer.Serialize(writer, new SerialisedExpression<T>(
              "number", null, null, e.Value,  null, null, null, null),
            options);
          return;

        case Expression.UnaryApplication<T> e:
          if (e.Operator is not Expression.UnaryOperator.Not)
            throw new JsonException();

          JsonSerializer.Serialize(writer, new SerialisedExpression<T>(
              "unary", null, null, null, "not", null, e.Argument, null),
            options);
          return;

        case Expression.BinaryApplication<T> e:
          var op = BinaryOperatorToString(e.Operator);

          JsonSerializer.Serialize(writer, new SerialisedExpression<T>(
              "binary", null, null, null, null, op, e.Left, e.Right),
            options);
          return;
      }
    }

    private Expression.BinaryOperator StringToBinaryOperator(
      string name)
    {
      if (name == "add") return new Expression.BinaryOperator.Add();
      if (name == "sub") return new Expression.BinaryOperator.Sub();
      if (name == "mult") return new Expression.BinaryOperator.Mult();
      if (name == "div") return new Expression.BinaryOperator.Div();

      if (name == "and") return new Expression.BinaryOperator.And();
      if (name == "or") return new Expression.BinaryOperator.Or();

      if (name == ">") return new Expression.BinaryOperator.Greater();
      if (name == ">=") return new Expression.BinaryOperator.GreaterOrEqual();
      if (name == "<") return new Expression.BinaryOperator.Less();
      if (name == "<=") return new Expression.BinaryOperator.LessOrEqual();
      if (name == "=") return new Expression.BinaryOperator.Equal();
      if (name == "!=") return new Expression.BinaryOperator.NotEqual();

      throw new JsonException();
    }

    private string BinaryOperatorToString(Expression.BinaryOperator op)
    {
      switch (op)
      {
        case Expression.BinaryOperator.Add: return "add";
        case Expression.BinaryOperator.Sub: return "sub";
        case Expression.BinaryOperator.Mult: return "mult";
        case Expression.BinaryOperator.Div: return "div";

        case Expression.BinaryOperator.And: return "and";
        case Expression.BinaryOperator.Or: return "or";

        case Expression.BinaryOperator.Greater: return ">";
        case Expression.BinaryOperator.GreaterOrEqual: return ">=";
        case Expression.BinaryOperator.Less: return "<";
        case Expression.BinaryOperator.LessOrEqual: return "<=";
        case Expression.BinaryOperator.Equal: return "=";
        case Expression.BinaryOperator.NotEqual: return "!=";
        default:
          throw new JsonException($"Invalid binary operator: {op}");
      }
    }
  }
}

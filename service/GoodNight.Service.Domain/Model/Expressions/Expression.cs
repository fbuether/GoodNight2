using System;

namespace GoodNight.Service.Domain.Model.Expressions
{
  /// <summary>
  /// Expressions are computable formulas involving Qualities. Their main use is
  /// to inspect a player's state and allow or disallow specific scenes based on
  /// that.
  /// </summary>
  /// <typeparam name="TQuality">
  /// The type of references to Qualities.
  /// </typeparam>
  public interface Expression<TQuality>
  {
    /// <summary>
    /// A Quality or a reference to a Quality.
    ///
    /// Depending on the processing step, this may be a string giving the
    /// Quality name, a IStoredReference to a Quality, or an actual Quality
    /// instance.
    /// </summary>
    public record Quality<Q>(
      Q Value)
      : Expression<Q>
    {
      public Value Evaluate(Func<Q, Value> context)
      {
        return context(Value);
      }

      public Expression<R> Map<R>(Func<Q, R> fun)
      {
        return new Quality(fun(Value));
      }
    }

    /// <summary>
    /// A literal boolean value, either true or false.
    /// </summary>
    public record Bool<Q>(
      bool Value)
      : Expression<Q>
    {
      public Value Evaluate(Func<Q, Value> context)
      {
        return new Value.Bool(Value);
      }

      public Expression<R> Map<R>(Func<Q, R> fun)
      {
        return (Expression<R>)this;
      }
    }

    /// <summary>
    /// A literal numerical value.
    /// </summary>
    public record Number<Q>(
      int Value)
      : Expression<Q>
    {
      public Value Evaluate(Func<Q, Value> context)
      {
        return new Value.Int(Value);
      }

      public Expression<R> Map<R>(Func<Q, R> fun)
      {
        return (Expression<R>)this;
      }
    }

    /// <summary>
    /// Operators that accept a single argument.
    /// </summary>
    public interface UnaryOperator
    {
      public record Not
        : UnaryOperator {}
    }

    /// <summary>
    /// A unary Operator applied to a single argument.
    /// </summary>
    public record UnaryApplication<Q>(
      UnaryOperator Operator,
      Expression<Q> Argument)
      : Expression<Q>
    {
      public Value Evaluate(Func<Q, Value> context)
      {
        var argVal = Argument.Evaluate(context);
        switch (Operator)
        {
          case UnaryOperator.Not:
            switch (argVal)
            {
              case Value.Bool v:
                return new Value.Bool(!v.Value);
              default:
                throw new TypeError($"Cannot apply {Operator} to {argVal}.");
            }
        }
      }

      public Expression<R> Map<R>(Func<Q, R> fun)
      {
        return new UnaryApplication(Operator, Argument.Map(fun));
      }
    }

    /// <summary>
    /// Operators that accept two arguments.
    /// </summary>
    public abstract record BinaryOperator
    {
      public record Add : BinaryOperator {}
      public record Sub : BinaryOperator {}
      public record Mult : BinaryOperator {}
      public record Div : BinaryOperator {}

      public record And : BinaryOperator {}
      public record Or : BinaryOperator {}

      public record Greater : BinaryOperator {}
      public record GreaterOrEqual : BinaryOperator {}
      public record Less : BinaryOperator {}
      public record LessOrEqual : BinaryOperator {}
      public record Equal : BinaryOperator {}
      public record NotEqual : BinaryOperator {}
    }

    public record BinaryApplication<Q>(
      BinaryOperator Operator,
      Expression<Q> Left,
      Expression<Q> Right)
      : Expression<Q>
    {
      private Value evaluateInts(int l, int r, Func<int, Value> toValue)
      {
        switch (Operator)
        {
          case BinaryOperator.Add: return toValue(l + r);
          case BinaryOperator.Sub: return toValue(l - r);
          case BinaryOperator.Mult: return toValue(l * r);
          case BinaryOperator.Div: return toValue(l / r);

          case BinaryOperator.Greater: return new Value.Bool(l > r);
          case BinaryOperator.GreaterOrEqual: return new Value.Bool(l >= r);
          case BinaryOperator.Less: return new Value.Bool(l < r);
          case BinaryOperator.LessOrEqual: return new Value.Bool(l <= r);
          case BinaryOperator.Equal: return new Value.Bool(l == r);
          case BinaryOperator.NotEqual: return new Value.Bool(l != r);
          default:
            throw new TypeError(
              $"Cannot apply ${Operator} to ({l},{r})");
        }
      }


      public Value Evaluate(Func<Q, Value> context)
      {
        var leftVal = Left.Evaluate(context);
        var rightVal = Right.Evaluate(context);

        switch (leftVal, rightVal)
        {
          case (Value.Bool(var l), Value.Bool(var r)):
            switch (Operator)
            {
              case BinaryOperator.And: return new Value.Bool(l && r);
              case BinaryOperator.Or: return new Value.Bool(l || r);
              case BinaryOperator.Equal: return new Value.Bool(l == r);
              case BinaryOperator.NotEqual: return new Value.Bool(l != r);
              default:
                throw new TypeError(
                  $"Cannot apply ${Operator} to ({leftVal},{rightVal})");
            }

          case (Value.Int(var l), Value.Int(var r)):
            return evaluateInts(l, r, i => new Value.Int(i));
          case (Value.Int(var l), Value.Enum(var r)):
            return evaluateInts(l, r, i => new Value.Int(i));

          case (Value.Enum(var l), Value.Int(var r)):
            return evaluateInts(l, r, i => new Value.Enum(i));
          case (Value.Enum(var l), Value.Enum(var r)):
            return evaluateInts(l, r, i => new Value.Enum(i));

          default:
            throw new TypeError(
              $"Cannot apply ${Operator} to ({leftVal},{rightVal})");
        }
      }

      public Expression<R> Map<R>(Func<Q, R> fun)
      {
        return new BinaryApplication(Operator, Left.Map(fun), Right.Map(fun));
      }
    }


    /// <summary>
    /// Evaluate this expression to a Value, given a specific context.
    /// The context must be able to provide a value for every kind of Quality
    /// or Quality reference, depending on what this Expression contains.
    /// </summary>
    /// <remarks>
    /// This may throw TypeError exceptions if the expression is not properly
    /// typed in the given context.
    /// </remarks>
    public Value Evaluate(Func<TQuality, Value> context);

    public Expression<R> Map<R>(Func<TQuality, R> fun);
  }
}

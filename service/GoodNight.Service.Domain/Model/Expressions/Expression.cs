using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using GoodNight.Service.Domain.Model.Read.Transfer;

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
    /// Evaluate this expression to a Value, given a specific context.
    /// The context must be able to provide a value for every kind of Quality
    /// or Quality reference, depending on what this Expression contains.
    /// </summary>
    /// <remarks>
    /// This may throw TypeError exceptions if the expression is not properly
    /// typed in the given context.
    /// </remarks>
    public Value Evaluate(Func<TQuality, Value> context, Random rnd);

    /// <summary>
    /// Transform the quality references of this expression to something
    /// different, using a mapper function.
    /// </summary>
    public Expression<R> Map<R>(Func<TQuality, R> fun);

    /// <summary>
    /// Format this expression to a string, given a way to format the
    /// qualities in the expression.
    /// </summary>
    public string Format(Func<TQuality, string> qualityToString);

    /// <summary>
    /// Collect all qualities in this expressions.
    /// </summary>
    public IEnumerable<TQuality> GetQualities();
  }

  public static class Expression
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
      public Value Evaluate(Func<Q, Value> context, Random rnd)
      {
        return context(Value);
      }

      public Expression<R> Map<R>(Func<Q, R> fun)
      {
        return new Quality<R>(fun(Value));
      }

      public string Format(Func<Q, string> qualityToString)
      {
        return qualityToString(Value);
      }

      public IEnumerable<Q> GetQualities()
      {
        yield return Value;
      }
    }

    /// <summary>
    /// A literal boolean value, either true or false.
    /// </summary>
    public record Bool<Q>(
      bool Value)
      : Expression<Q>
    {
      public Value Evaluate(Func<Q, Value> context, Random rnd)
      {
        return new Value.Bool(Value);
      }

      public Expression<R> Map<R>(Func<Q, R> fun)
      {
        return new Bool<R>(Value);
      }

      public string Format(Func<Q, string> qualityToString)
      {
        return Value ? "wahr" : "falsch";
      }

      public IEnumerable<Q> GetQualities()
      {
        yield break;
      }
    }

    /// <summary>
    /// A range of integer values, to be randomly determined at evaluation time.
    /// </summary>
    public record Range<Q>(
      Expression<Q> Lower,
      Expression<Q> Upper)
      : Expression<Q>
    {
      public Value Evaluate(Func<Q, Value> context, Random rnd)
      {
        var lower = Lower.Evaluate(context, rnd);
        var upper = Upper.Evaluate(context, rnd);

        if (lower is Value.Int l && upper is Value.Int u)
        {
          return new Value.Int(rnd.Next(l.Value, u.Value+1));
        }
        else
        {
          throw new TypeError("Range subexpressions do not contain numbers.");
        }
      }

      public Expression<R> Map<R>(Func<Q, R> fun)
      {
        return new Range<R>(Lower.Map(fun), Upper.Map(fun));
      }

      public string Format(Func<Q, string> qualityToString)
      {
        var low = Lower.Format(qualityToString);
        var up = Lower.Format(qualityToString);
        return $"[{low},{up}]";
      }

      public IEnumerable<Q> GetQualities()
      {
        return Lower.GetQualities().Concat(Upper.GetQualities());
      }
    }

    /// <summary>
    /// A literal numerical value.
    /// </summary>
    public record Number<Q>(
      int Value)
      : Expression<Q>
    {
      public Value Evaluate(Func<Q, Value> context, Random rnd)
      {
        return new Value.Int(Value);
      }

      public Expression<R> Map<R>(Func<Q, R> fun)
      {
        return new Number<R>(Value);
      }

      public string Format(Func<Q, string> qualityToString)
      {
        return Value.ToString();
      }

      public IEnumerable<Q> GetQualities()
      {
        yield break;
      }
    }

    /// <summary>
    /// Operators that accept a single argument.
    /// </summary>
    public interface UnaryOperator
    {
      public record Not
        : UnaryOperator
      {
        public override string ToString()
        {
          return "nicht";
        }
      }
    }

    /// <summary>
    /// A unary Operator applied to a single argument.
    /// </summary>
    public record UnaryApplication<Q>(
      UnaryOperator Operator,
      Expression<Q> Argument)
      : Expression<Q>
    {
      public Value Evaluate(Func<Q, Value> context, Random rnd)
      {
        var argVal = Argument.Evaluate(context, rnd);
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
          default:
            throw new InvalidOperationException($"Invalid operator {Operator}");
        }
      }

      public Expression<R> Map<R>(Func<Q, R> fun)
      {
        return new UnaryApplication<R>(Operator, Argument.Map(fun));
      }

      public string Format(Func<Q, string> qualityToString)
      {
        return Operator.ToString() + " " + Argument.Format(qualityToString);
      }

      public IEnumerable<Q> GetQualities()
      {
        return Argument.GetQualities();
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

      internal static string Format(BinaryOperator op)
      {
        switch (op)
        {
          case Add: return "+";
          case Sub: return "-";
          case Mult: return "*";
          case Div: return "/";

          case And: return "und";
          case Or: return "oder";

          case Greater: return ">";
          case GreaterOrEqual: return "≥";
          case Less: return "<";
          case LessOrEqual: return "≤";
          case Equal: return "=";
          case NotEqual: return "≠";
          default: return "<?>";
        }
      }
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


      public Value Evaluate(Func<Q, Value> context, Random rnd)
      {
        var leftVal = Left.Evaluate(context, rnd);
        var rightVal = Right.Evaluate(context, rnd);

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

          default:
            throw new TypeError(
              $"Cannot apply ${Operator} to ({leftVal},{rightVal})");
        }
      }

      public Expression<R> Map<R>(Func<Q, R> fun)
      {
        return new BinaryApplication<R>(Operator,
          Left.Map(fun), Right.Map(fun));
      }

      public string Format(Func<Q, string> qualityToString)
      {
        return Left.Format(qualityToString) + " " +
          BinaryOperator.Format(Operator) + " " +
          Right.Format(qualityToString);
      }

      public IEnumerable<Q> GetQualities()
      {
        return Left.GetQualities().Concat(Right.GetQualities());
      }
    }
  }
}

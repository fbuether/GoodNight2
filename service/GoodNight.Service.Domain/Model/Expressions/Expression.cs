
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
  public abstract record Expression<TQuality>
  {
    // a Quality refered to by name.
    public record Quality<Q>(
      Q Name)
      : Expression<Q> {}

    // Literals.
    public record Bool<Q>(
      bool Value)
      : Expression<Q> {}
    public record Number<Q>(
      int Value)
      : Expression<Q> {}

    // Operators with one argument
    public abstract record UnaryOperator
    {
      public record Not
        : UnaryOperator {}
    }

    // Application of a unary operator
    public record UnaryApplication<Q>(
      UnaryOperator Operator,
      Expression<Q> Argument)
      : Expression<Q> {}

    // Operators with two arguments
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

    // Application of a binary operator
    public record BinaryApplication<Q>(
      BinaryOperator Operator,
      Expression<Q> Left,
      Expression<Q> Right)
      : Expression<Q> {}
  }
}

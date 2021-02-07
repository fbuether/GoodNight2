
namespace GoodNight.Service.Domain.Model.Expressions
{
  public abstract record Expression
  {
    // a Quality refered to by name.
    public record Quality(
      string Name)
      : Expression {}

    // Literals.
    public record Bool(
      bool Value)
      : Expression {}
    public record Number(
      int Value)
      : Expression {}

    // Operators with one argument
    public abstract record UnaryOperator
    {
      public record Not
        : UnaryOperator {}
    }

    // Application of a unary operator
    public record UnaryApplication(
      UnaryOperator Operator,
      Expression Argument)
      : Expression {}

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
    public record BinaryApplication(
      BinaryOperator Operator,
      Expression Left,
      Expression Right)
      : Expression {}
  }
}

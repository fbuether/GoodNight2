using System;
using System.Linq;
using GoodNight.Service.Domain.Write.Expressions;
using Pidgin;


namespace GoodNight.Service.Domain.Parse
{
  using UnaryExprFun = Func<Expression, Expression>;
  using BinaryExprFun = Func<Expression, Expression, Expression>;

  public class ExpressionParser
  {
    private readonly static Parser<char, Expression> boolExpr =
      Parser.String("true")
      .WithResult<Expression>(new Expression.Bool(true))
      .Or(Parser.String("false")
        .WithResult<Expression>(new Expression.Bool(false)));

    private readonly static Parser<char, Expression> numberExpr =
      Parser.DecimalNum
      .Select<Expression>(num => new Expression.Number(num));

    private readonly static Parser<char, Expression> qualityExpr =
      NameParser.QualityName
      .Select<Expression>(name => new Expression.Quality(name));


    private static Parser<char, UnaryExprFun> unaryOp(
      Parser<char, Expression.UnaryOperator> op) =>
      op.Select<UnaryExprFun>(op => (expr) =>
        new Expression.UnaryApplication(op, expr));

    private static Parser<char, UnaryExprFun> buildUnary<T>(
      string rep, bool excludePostLetters)
      where T : Expression.UnaryOperator, new()
      {
        var parseName = Parser.String(rep);
        // if this operator is a possible quality name, we must make sure that
        // the word ends here.
        if (excludePostLetters) {
          parseName = parseName
            .Before(Parser.Not(Parser.Lookahead(NameParser.QualityLetters)));
        }

        return NameParser.InlineWhitespace
        .Then(unaryOp(
            Parser.Try(parseName)
            .WithResult<Expression.UnaryOperator>(new T()))
          .Before(NameParser.InlineWhitespace));
      }


    private static Parser<char, BinaryExprFun> binaryOp(
      Parser<char, Expression.BinaryOperator> op) =>
      op.Select<BinaryExprFun>(op => (exprLeft, exprRight) =>
        new Expression.BinaryApplication(op, exprLeft, exprRight));

    private static Parser<char, BinaryExprFun> buildBinary<T>(
      string rep)
      where T : Expression.BinaryOperator, new() =>
      buildBinaryParser<T, string>(Parser.String(rep));


    private static Parser<char, BinaryExprFun> buildBinaryParser<T, U>(
      Parser<char, U> op)
      where T : Expression.BinaryOperator, new() =>
      binaryOp(Parser.Try(op)
        .WithResult<Expression.BinaryOperator>(new T())
        .Before(NameParser.InlineWhitespace));

    private static Parser<char, BinaryExprFun> LessButNotUnequal =
      buildBinaryParser<Expression.BinaryOperator.Less, Unit>(
        Parser.String("<")
        .Then(Parser.Try(Parser.Not(Parser.String(">")))));

    private static Parser<char, Expression> bracedExpr(
      Parser<char, Expression> body) =>
      body
      .Between(Parser.String("(").Before(NameParser.InlineWhitespace),
        Parser.String(")"));


    internal readonly static Parser<char, Expression> expression =
      Pidgin.Expression.ExpressionParser.Build<char, Expression>(expr =>
        Parser.OneOf(
          numberExpr,
          qualityExpr.Labelled("Quality"),
          boolExpr,
          bracedExpr(expr).Labelled("Expression in braces")
        )
        .Before(NameParser.InlineWhitespace),
        new[] {
          Pidgin.Expression.Operator.PrefixChainable(
            buildUnary<Expression.UnaryOperator.Not>("not", true),
            buildUnary<Expression.UnaryOperator.Not>("!", false)),

          Pidgin.Expression.Operator.InfixL(
            buildBinary<Expression.BinaryOperator.Mult>("*")),
          Pidgin.Expression.Operator.InfixL(
            buildBinary<Expression.BinaryOperator.Div>("/")),
          Pidgin.Expression.Operator.InfixL(
            buildBinary<Expression.BinaryOperator.Sub>("-")),
          Pidgin.Expression.Operator.InfixL(
            buildBinary<Expression.BinaryOperator.Add>("+")),

          Pidgin.Expression.Operator.InfixN(
            buildBinary<Expression.BinaryOperator.GreaterOrEqual>(">=")),
          Pidgin.Expression.Operator.InfixN(
            buildBinary<Expression.BinaryOperator.Greater>(">")),
          Pidgin.Expression.Operator.InfixN(
            buildBinary<Expression.BinaryOperator.LessOrEqual>("<=")),
          Pidgin.Expression.Operator.InfixN(LessButNotUnequal),
          Pidgin.Expression.Operator.InfixN(
            buildBinary<Expression.BinaryOperator.Equal>("=")),
          Pidgin.Expression.Operator.InfixN(
            buildBinary<Expression.BinaryOperator.NotEqual>("!=")),
          Pidgin.Expression.Operator.InfixN(
            buildBinary<Expression.BinaryOperator.NotEqual>("<>")),

          Pidgin.Expression.Operator.InfixL(
            buildBinary<Expression.BinaryOperator.And>("and")),
          Pidgin.Expression.Operator.InfixL(
            buildBinary<Expression.BinaryOperator.Or>("or")),
        });



    public ParseResult<Expression> Parse(string input)
    {
      var res =
        NameParser.InlineWhitespace // leading whitespace
        .Then(expression)
        .Before(Parser<char>.End)
        .Parse(input);

      return new ParseResult<Expression>(res.Success,
        res.Success ? res.Value : null,
        !res.Success && res.Error is not null
        ? res.Error.Message
        : null,
        !res.Success && res.Error is not null
        ? new Tuple<int,int>(res.Error.ErrorPos.Line, res.Error.ErrorPos.Col)
        : null,
        !res.Success && res.Error is not null && res.Error.Unexpected.HasValue
        ? res.Error.Unexpected.Value.ToString()
        : null,
        !res.Success && res.Error is not null
        ? String.Join(", ", res.Error.Expected.Select(e => e.ToString()))
        : null);
    }
  }
}

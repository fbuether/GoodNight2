using System;
using System.Linq;
using GoodNight.Service.Domain.Model.Expressions;
using Pidgin;


namespace GoodNight.Service.Domain.Parse
{
  using Expr = Expression<string>;

  using UnaryExprFun = Func<Expression<string>, Expression<string>>;
  using BinaryExprFun = Func<Expression<string>, Expression<string>,
    Expression<string>>;

  public class ExpressionParser
  {
    private readonly static Parser<char, Expr> boolExpr =
      Parser.String("true")
      .WithResult<Expr>(new Expr.Bool<string>(true))
      .Or(Parser.String("false")
        .WithResult<Expr>(new Expr.Bool<string>(false)));

    private readonly static Parser<char, Expr> numberExpr =
      Parser.DecimalNum
      .Select<Expr>(num => new Expr.Number<string>(num));

    private readonly static Parser<char, Expr> qualityExpr =
      NameParser.QualityName
      .Select<Expr>(name => new Expr.Quality<string>(name));


    private static Parser<char, UnaryExprFun> unaryOp(
      Parser<char, Expr.UnaryOperator> op) =>
      op.Select<UnaryExprFun>(op => (expr) =>
        new Expr.UnaryApplication<string>(op, expr));

    private static Parser<char, UnaryExprFun> buildUnary<T>(
      string rep, bool excludePostLetters)
      where T : Expr.UnaryOperator, new()
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
            .WithResult<Expr.UnaryOperator>(new T()))
          .Before(NameParser.InlineWhitespace));
      }


    private static Parser<char, BinaryExprFun> binaryOp(
      Parser<char, Expr.BinaryOperator> op) =>
      op.Select<BinaryExprFun>(op => (exprLeft, exprRight) =>
        new Expr.BinaryApplication<string>(op, exprLeft, exprRight));

    private static Parser<char, BinaryExprFun> buildBinary<T>(
      string rep)
      where T : Expr.BinaryOperator, new() =>
      buildBinaryParser<T, string>(Parser.String(rep));


    private static Parser<char, BinaryExprFun> buildBinaryParser<T, U>(
      Parser<char, U> op)
      where T : Expr.BinaryOperator, new() =>
      binaryOp(Parser.Try(op)
        .WithResult<Expr.BinaryOperator>(new T())
        .Before(NameParser.InlineWhitespace));

    private static Parser<char, BinaryExprFun> LessButNotUnequal =
      buildBinaryParser<Expr.BinaryOperator.Less, Unit>(
        Parser.String("<")
        .Then(Parser.Try(Parser.Not(Parser.String(">")))));

    private static Parser<char, Expr> bracedExpr(
      Parser<char, Expr> body) =>
      body
      .Between(Parser.String("(").Before(NameParser.InlineWhitespace),
        Parser.String(")"));


    internal readonly static Parser<char, Expr> Expression =
      Pidgin.Expression.ExpressionParser.Build<char, Expr>(expr =>
        Parser.OneOf(
          numberExpr,
          qualityExpr.Labelled("Quality"),
          boolExpr,
          bracedExpr(expr).Labelled("Expr in braces")
        )
        .Before(NameParser.InlineWhitespace),
        new[] {
          Pidgin.Expression.Operator.PrefixChainable(
            buildUnary<Expr.UnaryOperator.Not>("not", true),
            buildUnary<Expr.UnaryOperator.Not>("!", false)),

          Pidgin.Expression.Operator.InfixL(
            buildBinary<Expr.BinaryOperator.Mult>("*")),
          Pidgin.Expression.Operator.InfixL(
            buildBinary<Expr.BinaryOperator.Div>("/")),
          Pidgin.Expression.Operator.InfixL(
            buildBinary<Expr.BinaryOperator.Sub>("-")),
          Pidgin.Expression.Operator.InfixL(
            buildBinary<Expr.BinaryOperator.Add>("+")),

          Pidgin.Expression.Operator.InfixN(
            buildBinary<Expr.BinaryOperator.GreaterOrEqual>(">=")),
          Pidgin.Expression.Operator.InfixN(
            buildBinary<Expr.BinaryOperator.Greater>(">")),
          Pidgin.Expression.Operator.InfixN(
            buildBinary<Expr.BinaryOperator.LessOrEqual>("<=")),
          Pidgin.Expression.Operator.InfixN(LessButNotUnequal),
          Pidgin.Expression.Operator.InfixN(
            buildBinary<Expr.BinaryOperator.Equal>("=")),
          Pidgin.Expression.Operator.InfixN(
            buildBinary<Expr.BinaryOperator.NotEqual>("!=")),
          Pidgin.Expression.Operator.InfixN(
            buildBinary<Expr.BinaryOperator.NotEqual>("<>")),

          Pidgin.Expression.Operator.InfixL(
            buildBinary<Expr.BinaryOperator.And>("and")),
          Pidgin.Expression.Operator.InfixL(
            buildBinary<Expr.BinaryOperator.Or>("or")),
        });



    public ParseResult<Expr> Parse(string input)
    {
      var res =
        NameParser.InlineWhitespace // leading whitespace
        .Then(Expression)
        .Before(Parser<char>.End)
        .Parse(input);

      return res.Success
        ? new ParseResult.Success<Expr>(res.Value)
        : ParseResult.Failure<Expr>.OfError(res.Error);
    }
  }
}

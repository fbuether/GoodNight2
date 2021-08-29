using System;
using GoodNight.Service.Domain.Model.Expressions;
using Pidgin;

namespace GoodNight.Service.Domain.Parse
{
  using UnaryExprFun = Func<Expression<string>, Expression<string>>;
  using BinaryExprFun = Func<Expression<string>, Expression<string>,
    Expression<string>>;

  public static class ExpressionParser
  {
    private readonly static Parser<char, Expression<string>> boolExpr =
      Parser.String("true")
      .WithResult<Expression<string>>(new Expression.Bool<string>(true))
      .Or(Parser.String("false")
        .WithResult<Expression<string>>(new Expression.Bool<string>(false)));

    private readonly static Parser<char, Expression<string>> numberExpr =
      Parser.DecimalNum
      .Select<Expression<string>>(num => new Expression.Number<string>(num));

    private readonly static Parser<char, Expression<string>> qualityExpr =
      NameParser.QualityName
      .Select<Expression<string>>(name => new Expression.Quality<string>(name));

    private static Parser<char, Expression<string>> rangeExpr(
      Parser<char, Expression<string>> exprParser) =>
      exprParser.Between(NameParser.InlineWhitespace)
      .Before(Parser.Char(','))
      .Then<Expression<string>, Expression<string>>(
        exprParser.Between(NameParser.InlineWhitespace),
        (a,b) => new Expression.Range<string>(a, b))
      .Between(Parser.Char('['), Parser.Char(']'));

    private static Parser<char, UnaryExprFun> unaryOp(
      Parser<char, Expression.UnaryOperator> op) =>
      op.Select<UnaryExprFun>(op => (expr) =>
        new Expression.UnaryApplication<string>(op, expr));

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
        new Expression.BinaryApplication<string>(op, exprLeft, exprRight));

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

    private static Parser<char, Expression<string>> bracedExpr(
      Parser<char, Expression<string>> body) =>
      body
      .Between(Parser.String("(").Before(NameParser.InlineWhitespace),
        Parser.String(")"));


    internal readonly static Parser<char, Expression<string>> Expression =
      Pidgin.Expression.ExpressionParser.Build<char, Expression<string>>(expr =>
        Parser.OneOf(
          numberExpr.Labelled("Number"),
          qualityExpr.Labelled("Quality"),
          rangeExpr(expr).Labelled("Range"),
          boolExpr.Labelled("Boolean"),
          bracedExpr(expr).Labelled("Braced Expr")
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


    public static ParseResult<Expression<string>> Parse(string input)
    {
      var res = Expression
        .Between(NameParser.InlineWhitespace) // allow surrounding whitespace.
        .Before(Parser<char>.End)
        .Parse(input);

      return res.Success
        ? new ParseResult.Success<Expression<string>>(res.Value)
        : ParseResult.Failure<Expression<string>>.OfError(res.Error);
    }
  }
}

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
      // Parser<char>.FromResult(Unit.Value).Trace(f => $"-- debut \"{f}\" starting bool")
      // .Then(
      Parser.String("true")// .Trace(f => $"-- debut \"{f}\" parsed true")
      .WithResult<Expression>(new Expression.Bool(true))
      .Or(Parser.String("false")
        .WithResult<Expression>(new Expression.Bool(false)))
      // .Trace(f => $"-- debut \"{f}\" parsed boolExpr")
      // )
    ;

    private readonly static Parser<char, Expression> numberExpr =
      // Parser<char>.FromResult(Unit.Value).Trace(f => $"-- debut \"{f}\" starting number")
      // .Then(
        Parser.DecimalNum// .Trace(f => $"-- debut \"{f}\" within the number expr parser")
      .Select<Expression>(num => new Expression.Number(num))
      // )
    ;

    private readonly static Parser<char, Expression> qualityExpr =
      // Parser<char>.FromResult(Unit.Value).Trace(f => $"-- debut \"{f}\" starting quality")
      // .Then(
      NameParser.QualityName// .Trace(f => $"-- debut \"{f}\" within quality expr")
      .Select<Expression>(name => new Expression.Quality(name))
      // .Trace(f => $"-- debug \"{f}\" finished quailty")
      // )
    ;



    private static Parser<char, UnaryExprFun> unaryOp(
      Parser<char, Expression.UnaryOperator> op) =>
      op.Select<UnaryExprFun>(op => (expr) =>
        new Expression.UnaryApplication(op, expr));

    private static Parser<char, UnaryExprFun> buildUnary<T>(
      string rep, bool excludePostLetters)
      where T : Expression.UnaryOperator, new()
      {
        var parseName = Parser.String(rep);
        if (excludePostLetters) {
          parseName = parseName
            .Before(
              Parser.Not(Parser.Lookahead(NameParser.QualityLetters)));
        }

        return NameParser.InlineWhitespace
        .Then(unaryOp(
            Parser.Try(parseName)
            .WithResult<Expression.UnaryOperator>(new T()))
          .Before(NameParser.InlineWhitespace)
          // .Trace(f => $"-- debut \"{f}\" finished a unary operator")
        );
      }


    private static Parser<char, BinaryExprFun> binaryOp(
      Parser<char, Expression.BinaryOperator> op) =>
      op.Select<BinaryExprFun>(op => (exprLeft, exprRight) =>
        new Expression.BinaryApplication(op, exprLeft, exprRight));

    private static Parser<char, BinaryExprFun> buildBinary<T>(
      string rep)
      where T : Expression.BinaryOperator, new() =>
      binaryOp(Parser.Try(Parser.String(rep))
        .WithResult<Expression.BinaryOperator>(new T())
        .Before(NameParser.InlineWhitespace));



    private static Parser<char, Expression> bracedExpr(
      Parser<char, Expression> body) =>
      // Parser<char>.FromResult(Unit.Value).Trace(f => $"-- debut \"{f}\" starting braced")
      // .Then(
      body// .Trace(f => $"-- debut \"{f}\" parsed braced body")
      .Between(
        Parser.String("(").Before(NameParser.InlineWhitespace)

// .Trace(f => $"-- debut \"{f}\" starting braced parse")
        ,
        Parser.String(")"))// .Trace(f => $"-- debut \"{f}\" parsed a braced thing")
      // )
    ;


    internal readonly static Parser<char, Expression> expression =

//               Parser<char>.FromResult(Unit.Value).Trace(f => $"-- debut \"{f}\" starting expression")
//         .Then(Parser.Lookahead(Parser<char>.Any.ManyString()).Trace(f => $"-- debut \"{f}\" remainder"))
// .Then(
      Pidgin.Expression.ExpressionParser.Build<char, Expression>(
        expr =>

//               Parser<char>.FromResult(Unit.Value).Trace(f => $"-- debut \"{f}\" starting inner expression")
//         .Then(Parser.Lookahead(Parser<char>.Any.ManyString()).Trace(f => $"-- debut \"{f}\" remainder"))

//         // .Then(Parser.Lookahead(Parser<char>.Any.ManyString()).Trace(f => $"-- debut \"{f}\" remainder"))

// .Then(
  Parser.OneOf(
          numberExpr,
          qualityExpr.Labelled("Quality"),
          boolExpr,
          bracedExpr(expr).Labelled("Expression in braces")
        )
  .Before(NameParser.InlineWhitespace)
        // .Trace(f => $"-- debut \"{f}\" finished an inner expression")
// )

,
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
          Pidgin.Expression.Operator.InfixN(
            buildBinary<Expression.BinaryOperator.Less>("<")),
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
        }
// )
);





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

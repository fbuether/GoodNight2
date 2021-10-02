using System;
using System.Collections.Generic;
using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Domain.Parse;
using Xunit;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick;

namespace GoodNight.Service.Domain.Test.Parse
{
  using UnaryApplication = Expression.UnaryApplication<string>;
  using BinaryApplication = Expression.BinaryApplication<string>;
  using Bool = Expression.Bool<string>;
  using Number = Expression.Number<string>;
  using Range = Expression.Range<string>;
  using Quality = Expression.Quality<string>;

  [FeatureFile("Parse/ExpressionParserTest.feature")]
  public class ExpressionParserTest : Xunit.Gherkin.Quick.Feature
  {
    private ITestOutputHelper output;

    public ExpressionParserTest(ITestOutputHelper output)
    {
      this.output = output;
    }

    private IEnumerable<Expression<string>> FindAllNodes(
      Expression<string> node)
    {
      yield return node;

      if (node is UnaryApplication u)
      {
        foreach (var expr in FindAllNodes(u.Argument))
        {
          yield return expr;
        }
      }

      if (node is BinaryApplication b)
      {
        foreach (var expr in FindAllNodes(b.Left))
        {
          yield return expr;
        }

        foreach (var expr in FindAllNodes(b.Right))
        {
          yield return expr;
        }
      }
    }


    private string? input;

    private ParseResult<Expression<string>>? result;

    [Given(@"the input (.*)")]
    public void TheExpression(string input)
    {
      this.input = input;
      output.WriteLine($"input is: --------8<--------");
      output.WriteLine(input);
      output.WriteLine("--------8<--------");
    }

    [When("the parser parses the input")]
    public void TheParserParsesTheInput()
    {
      Assert.NotNull(input);
      result = ExpressionParser.Parse(input!);

      // to debug failing tests.
      switch (result) {
        case ParseResult.Success<Expression<string>> r:
          output.WriteLine($"Result is sucess.");
          output.WriteLine($"Value: {r.Result}.");
          output.WriteLine("Result:");
          foreach (var node in FindAllNodes(r.Result))
          {
            output.WriteLine($"  node: {node}");
          }

          break;
        case ParseResult.Failure<Expression<string>> r:
          output.WriteLine($"Result is failure.");
          output.WriteLine($"ErrorMessage: {r.ErrorMessage}");
          output.WriteLine($"ErrorPosition: {r.ErrorPosition}");
          output.WriteLine($"UnexpectedToken: {r.UnexpectedToken}");
          output.WriteLine($"ExpectedToken: {r.ExpectedToken}");
          break;
      }
    }

    [Then("parsing fails")]
    public void ParsingFails()
    {
      Assert.NotNull(result);
      Assert.IsType<ParseResult.Failure<Expression<string>>>(result);
    }

    [Then("parsing succeeds")]
    public void ParsingSucceeds()
    {
      Assert.NotNull(result);
      Assert.IsType<ParseResult.Success<Expression<string>>>(result);
      Assert.NotNull((result as ParseResult.Success<Expression<string>>)!
        .Result);
    }

    private Expression<string> Get(ParseResult<Expression<string>>? result)
    {
      Assert.NotNull(result);
      Assert.IsType<ParseResult.Success<Expression<string>>>(result!);

      switch (result!) {
        case ParseResult.Success<Expression<string>> r:
          return r.Result;
      }

      throw new Exception();
    }

    // helper tools to build reference expressions.
    private static Expression<string> expT = new Bool(true);
    private static Expression<string> expF = new Bool(false);
    private static Expression<string> exp7 = new Number(7);
    private static Expression<string> exp9 = new Number(9);

    private static Expression<string> mkNot(Expression<string> op) =>
      new UnaryApplication(new Expression.UnaryOperator.Not(), op);
    private static Expression<string> mkBin(Expression.BinaryOperator op,
      Expression<string> l, Expression<string> r) =>
      new BinaryApplication(op, l, r);

    private static Expression<string> mkAnd(Expression<string> l,
      Expression<string> r) =>
      mkBin(new Expression.BinaryOperator.And(), l, r);
    private static Expression<string> mkOr(Expression<string> l,
      Expression<string> r) =>
      mkBin(new Expression.BinaryOperator.Or(), l, r);

    private static Dictionary<string, Expression<string>> results =
      new Dictionary<string, Expression<string>>() {
      { "true", expT },
      { "false", expF },
      { "-14", new Number(-14) },
      { "7", exp7 },
      { "77516", new Number(77516) },
      { "[4..13]", new Range(new Number(4), new Number(13)) },
      { "[-4..-13]", new Range(new Number(-4), new Number(-13)) },
      { "[0..9+7]", new Range(new Number(0), mkBin(new Expression.BinaryOperator.Add(), exp9, exp7)) },

      { "Quality \"qualityname\"", new Quality("qualityname") },
      { "Quality \"quality name\"", new Quality("quality name") },
      { "Quality \"quality name with several parts\"",
        new Quality("quality name with several parts") },
      { "Quality \"not\"", new Quality("not") },
      { "Quality \"notable\"", new Quality("notable") },
      { "Quality \"orwellian\"", new Quality("orwellian") },

      { "Quality \"this\" and Quality \"that\"",
        mkAnd(new Quality("this"), new Quality("that")) },
      { "false and Quality \"that\"",
        mkAnd(expF, new Quality("that")) },

      { "not true", mkNot(expT) },
      { "not not true", mkNot(mkNot(expT)) },
      { "true and true", mkAnd(expT, expT) },
      { "true and false", mkAnd(expT, expF) },
      { "true or false", mkOr(expT, expF) },
      { "true or not true", mkOr(expT, mkNot(expT)) },
      { "(true and true) and true", mkAnd(mkAnd(expT, expT), expT) },
      { "true and (true and true)", mkAnd(expT, mkAnd(expT, expT)) },
      { "(true and true) or true", mkOr(mkAnd(expT, expT), expT) },
      { "true or (true and true)", mkOr(expT, mkAnd(expT, expT)) },

      { "7 + 9", mkBin(new Expression.BinaryOperator.Add(), exp7, exp9) },
      { "7 - 9", mkBin(new Expression.BinaryOperator.Sub(), exp7, exp9) },
      { "7 * 9", mkBin(new Expression.BinaryOperator.Mult(), exp7, exp9) },
      { "7 / 9", mkBin(new Expression.BinaryOperator.Div(), exp7, exp9) },

      { "7 < 9", mkBin(new Expression.BinaryOperator.Less(), exp7, exp9) },
      { "7 <= 9",
        mkBin(new Expression.BinaryOperator.LessOrEqual(), exp7, exp9) },
      { "7 > 9", mkBin(new Expression.BinaryOperator.Greater(), exp7, exp9) },
      { "7 >= 9",
        mkBin(new Expression.BinaryOperator.GreaterOrEqual(), exp7, exp9) },
      { "7 = 9", mkBin(new Expression.BinaryOperator.Equal(), exp7, exp9) },
      { "7 != 9", mkBin(new Expression.BinaryOperator.NotEqual(), exp7, exp9) },
      { "7 <> 9", mkBin(new Expression.BinaryOperator.NotEqual(), exp7, exp9) },

    };

    [Then("the result is (.*)")]
    public void TheResultIs(string val)
    {
      var expr = results[val];
      if (expr is null)
      {
        throw new ArgumentException($"result string \"{val}\" not known.");
      }

      Assert.Equal(expr, Get(result));
    }
  }
}

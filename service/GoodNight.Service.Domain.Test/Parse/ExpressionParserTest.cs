using System;
using System.Collections.Generic;
using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Domain.Parse;
using Xunit;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick;

namespace GoodNight.Service.Domain.Test.Parse
{
  using Expression = Expression<string>;
  using UnaryApplication = Expression<string>.UnaryApplication<string>;
  using BinaryApplication = Expression<string>.BinaryApplication<string>;
  using Bool = Expression<string>.Bool<string>;
  using Number = Expression<string>.Number<string>;
  using Quality = Expression<string>.Quality<string>;

  [FeatureFile("Parse/ExpressionParserTest.feature")]
  public class ExpressionParserTest : Xunit.Gherkin.Quick.Feature
  {
    private ITestOutputHelper output;

    public ExpressionParserTest(ITestOutputHelper output)
    {
      this.output = output;
    }

    private IEnumerable<Expression> FindAllNodes(Expression node)
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

    private ParseResult<Expression>? result;

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
      result = new ExpressionParser().Parse(input!);

      // to debug failing tests.
      output.WriteLine($"IsSuccessful: {result!.IsSuccessful}");
      output.WriteLine("Result:");
      if (result is not null && result.Result is not null)
      {
        foreach (var node in FindAllNodes(result.Result))
        {
          output.WriteLine($"  node: {node}");
        }
      }

      output.WriteLine($"ErrorMessage: {result!.ErrorMessage}");
      output.WriteLine($"ErrorPosition: {result!.ErrorPosition}");
      output.WriteLine($"UnexpectedToken: {result!.UnexpectedToken}");
      output.WriteLine($"ExpectedToken: {result!.ExpectedToken}");
    }

    [Then("parsing fails")]
    public void ParsingFails()
    {
      Assert.NotNull(result);
      Assert.False(result!.IsSuccessful);
    }

    [Then("parsing succeeds")]
    public void ParsingSucceeds()
    {
      Assert.NotNull(result);
      Assert.True(result!.IsSuccessful);
    }

    // helper tools to build reference expressions.
    private static Expression expT = new Bool(true);
    private static Expression expF = new Bool(false);
    private static Expression exp7 = new Number(7);
    private static Expression exp9 = new Number(9);

    private static Expression mkNot(Expression op) =>
      new UnaryApplication(new Expression.UnaryOperator.Not(), op);
    private static Expression mkBin(Expression.BinaryOperator op, Expression l,
      Expression r) =>
      new BinaryApplication(op, l, r);

    private static Expression mkAnd(Expression l, Expression r) =>
      mkBin(new Expression.BinaryOperator.And(), l, r);
    private static Expression mkOr(Expression l, Expression r) =>
      mkBin(new Expression.BinaryOperator.Or(), l, r);

    private static Dictionary<string, Expression> results =
      new Dictionary<string, Expression>() {
      { "true", expT },
      { "false", expF },
      { "-14", new Number(-14) },
      { "7", exp7 },
      { "77516", new Number(77516) },
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

      Assert.Equal(expr, result!.Result);
    }
  }
}

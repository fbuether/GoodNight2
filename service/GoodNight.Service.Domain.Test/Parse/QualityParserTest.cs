using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Gherkin.Ast;
using GoodNight.Service.Domain.Model;
using GoodNight.Service.Domain.Model.Write;
using GoodNight.Service.Domain.Parse;
using Xunit;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick;

namespace GoodNight.Service.Domain.Test.Parse
{
  [FeatureFile("Parse/QualityParserTest.feature")]
  public class QualityParserTest : Xunit.Gherkin.Quick.Feature
  {
    private ITestOutputHelper output;

    public QualityParserTest(ITestOutputHelper output)
    {
      this.output = output;
    }


    private string? input;

    private ParseResult<Quality>? result;



    [Given("the quality input")]
    public void TheQualityInput(DocString body)
    {
      input = body.Content;
      output.WriteLine($"input is ({input.Length}): --------8<--------");
      output.WriteLine(input);
      output.WriteLine("--------8<--------");
    }

    [When("the parser parses the input")]
    public void TheParserParsesTheInput()
    {
      Assert.NotNull(input);
      result = QualityParser.Parse(input!)!;

      // to debug failing tests.
      switch (result) {
        case ParseResult.Success<Quality> r:
          output.WriteLine($"Result is sucess.");
          output.WriteLine($"Value: {r.Result}.");
          break;
        case ParseResult.Failure<Quality> r:
          output.WriteLine($"Result is failure.");
          output.WriteLine($"ErrorMessage: {r.ErrorMessage}.");
          output.WriteLine($"ErrorPosition: {r.ErrorPosition}.");
          output.WriteLine($"UnexpectedToken: {r.UnexpectedToken}.");
          output.WriteLine($"ExpectedToken: {r.ExpectedToken}.");
          break;
      }
    }


    [Then("parsing fails")]
    public void ParsingFails()
    {
      Assert.NotNull(result);
      Assert.IsType<ParseResult.Failure<Quality>>(result);
    }

    [Then("parsing succeeds")]
    public void ParsingSucceeds()
    {
      Assert.NotNull(result);
      Assert.IsType<ParseResult.Success<Quality>>(result);
      Assert.NotNull((result as ParseResult.Success<Quality>)!.Result);
    }

    public Quality Get(ParseResult<Quality>? result)
    {
      Assert.NotNull(result);
      Assert.IsType<ParseResult.Success<Quality>>(result!);
 
      switch (result!) {
        case ParseResult.Success<Quality> r:
          return r.Result;
      }

      throw new Exception();
    }

    [Then(@"the quality has name ""(.*)""")]
    public void TheQualityHasNameString(string name)
    {
      Assert.Equal(name, Get(result).Name);
    }

    [Then(@"the quality has description ""(.*)""")]
    public void TheQualityHasDescriptionString(string description)
    {
      Assert.Equal(description, Get(result).Description);
    }

    [Then("the quality has no scene")]
    public void TheQualityHasNoScene()
    {
      Assert.Null(Get(result).Scene);
    }

    [Then(@"the quality has type (.*)")]
    public void TheQualityHasTypeName(string typeName)
    {
      Assert.Equal(typeName, Get(result).GetType().Name);
    }

    [Then(@"the quality is( not)? hidden")]
    public void TheQualityIsMaybeHidden(string not)
    {
      if (not == " not")
      {
        Assert.False(Get(result).Hidden);
      }
      else
      {
        Assert.True(Get(result).Hidden);
      }
    }

    [Then(@"the quality has minimum (\d+)")]
    public void TheQualityHasMinimumValue(int min)
    {
      Assert.IsType<Quality.Int>(Get(result));
      Assert.Equal(min, (Get(result) as Quality.Int)!.Minimum);
    }

    [Then(@"the quality has maximum (\d+)")]
    public void TheQualityHasMaximumValue(int max)
    {
      Assert.IsType<Quality.Int>(Get(result));
      Assert.Equal(max, (Get(result) as Quality.Int)!.Maximum);
    }

    [Then(@"the quality has level (\d+) with text (.*)")]
    public void TheQualityHasLevelNWithTextString(int level, string text)
    {
      Assert.IsType<Quality.Enum>(Get(result));
      var e = Get(result) as Quality.Enum;
      Assert.True(e!.Levels.ContainsKey(level));
      Assert.Equal(text, e!.Levels[level]);
    }


    [Then(@"the quality has scene (.*)")]
    public void TheQualityHasSceneName(string name)
    {
      Assert.Equal(name, Get(result).Scene);
    }

    [Then(@"the raw text is")]
    public void TheRawTextIs(DocString body)
    {
      var raw = body.Content;
      Assert.Equal(raw, Get(result).Raw);
    }
  }
}

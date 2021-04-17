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
      result = QualityParser.Parse(input!);

      // to debug failing tests.
      output.WriteLine($"IsSuccessful: {result!.IsSuccessful}");
      output.WriteLine($"Result: {result!.Result}");
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
      Assert.NotNull(result!.Result);
    }

    [Then(@"the quality has name ""(.*)""")]
    public void TheQualityHasNameString(string name)
    {
      Assert.Equal(name, result!.Result!.Name);
    }

    [Then(@"the quality has description ""(.*)""")]
    public void TheQualityHasDescriptionString(string description)
    {
      Assert.Equal(description, result!.Result!.Description);
    }

    [Then("the quality has no scene")]
    public void TheQualityHasNoScene()
    {
      Assert.Null(result!.Result!.Scene);
    }

    [Then(@"the quality has type (.*)")]
    public void TheQualityHasTypeName(string typeName)
    {
      Assert.Equal(typeName, result!.Result!.GetType().Name);
    }

    [Then(@"the quality is( not)? hidden")]
    public void TheQualityIsMaybeHidden(string not)
    {
      if (not == " not")
      {
        Assert.False(result!.Result!.Hidden);
      }
      else
      {
        Assert.True(result!.Result!.Hidden);
      }
    }

    [Then(@"the quality has minimum (\d+)")]
    public void TheQualityHasMinimumValue(int min)
    {
      Assert.IsType<Quality.Int>(result!.Result!);
      Assert.Equal(min, (result!.Result as Quality.Int)!.Minimum);
    }

    [Then(@"the quality has maximum (\d+)")]
    public void TheQualityHasMaximumValue(int max)
    {
      Assert.IsType<Quality.Int>(result!.Result!);
      Assert.Equal(max, (result!.Result as Quality.Int)!.Maximum);
    }

    [Then(@"the quality has level (\d+) with text (.*)")]
    public void TheQualityHasLevelNWithTextString(int level, string text)
    {
      Assert.IsType<Quality.Enum>(result!.Result!);
      var e = result!.Result as Quality.Enum;
      Assert.True(e!.Levels.ContainsKey(level));
      Assert.Equal(text, e!.Levels[level]);
    }


    [Then(@"the quality has scene (.*)")]
    public void TheQualityHasSceneName(string name)
    {
      Assert.Equal(name, result!.Result!.Scene);
    }

    [Then(@"the raw text is")]
    public void TheRawTextIs(DocString body)
    {
      var raw = body.Content;
      Assert.Equal(raw, result!.Result!.Raw);
    }
  }
}

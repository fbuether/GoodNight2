using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Gherkin.Ast;
using GoodNight.Service.Domain.Model.Parse;
using GoodNight.Service.Domain.Parse;
using Xunit;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick;
using Content = GoodNight.Service.Domain.Model.Parse.Quality.Content;
using ExprType = GoodNight.Service.Domain.Model.Expressions.Type;

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

    private IImmutableList<Quality.Content>? contents;


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
      switch (result) {
        case ParseResult.Success<Quality> r:
          output.WriteLine($"Result is sucess.");
          output.WriteLine($"Value: {r.Result}.");
          foreach (var content in r.Result.Contents)
          {
            output.WriteLine($"Content: {content}");
          }
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

      contents = (result as ParseResult.Success<Quality>)?.Result.Contents;
      Assert.NotNull(contents);
    }

    private T GetAs<T>(int number)
      where T : Content
    {
      Assert.True(contents!.Count >= number);
      Assert.IsType<T>(contents[number-1]);
      return (contents[number-1] as T)!;
    }

    [Then(@"there is (\d) content")]
    public void ThereIsNumberContent(int count)
    {
      Assert.Equal(count, contents!.Count);
    }

    [Then(@"content (\d) is a text with value ""(.*)""")]
    public void ContentNumberIsATextWithValueString(int number, string text)
    {
      Assert.Equal(text, GetAs<Content.Text>(number).Value);
    }

    [Then(@"content (\d) is a name with value ""(.*)""")]
    public void ContentNumberIsANameWithValueString(int number, string name)
    {
      Assert.Equal(name, GetAs<Content.Name>(number).Value);
    }

    [Then(@"content (\d) is a type of (bool|int|enum)")]
    public void ContentNumberIsATypeOfBoolIntEnum(int number, string type)
    {
      var target = type == "bool"
        ? ExprType.Bool
        : type == "int"
        ? ExprType.Int
        : ExprType.Enum;

      Assert.Equal(target, GetAs<Content.Type>(number).Value);
    }

    [Then("no content is hidden")]
    public void NoContentIsHidden()
    {
      Assert.All(contents, Assert.IsNotType<Content.Hidden>);
    }

    [Then(@"content (\d) is hidden")]
    public void ContentNumberIsHidden(int number)
    {
      GetAs<Content.Hidden>(number);
    }

    [Then("no content is scene")]
    public void NoContentIsScene()
    {
      Assert.All(contents, Assert.IsNotType<Content.Scene>);
    }

    [Then(@"content (\d) is scene with name ""(.*)""")]
    public void ContentNumberIsSceneWithNameString(int number, string scene)
    {
      Assert.Equal(scene, GetAs<Content.Scene>(number).Urlname);
    }

    [Then(@"content (\d) is a level of number (\d) and text ""(.*)""")]
    public void ContentNumberIsALevelOfNumberNumberAndTextString(
      int content, int number, string caption)
    {
      Assert.Equal(number, GetAs<Content.Level>(content).Number);
      Assert.Equal(caption, GetAs<Content.Level>(content).Description);
    }

    [Then(@"content (\d) is a minimum of (\d)")]
    public void ContentNumberIsAMinimumOfNumber(int number, int min)
    {
      Assert.Equal(min, GetAs<Content.Minimum>(number).Value);
    }

    [Then(@"content (\d) is a maximum of (\d)")]
    public void ContentNumberIsAMaximumOfNumber(int number, int max)
    {
      Assert.Equal(max, GetAs<Content.Maximum>(number).Value);
    }
  }
}

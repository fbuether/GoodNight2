using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Gherkin.Ast;
using GoodNight.Service.Domain.Parse;
using GoodNight.Service.Domain.Write;
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
      result = new QualityParser().Parse(input!);

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
  }
}

using System.Collections.Generic;
using System.Linq;
using Gherkin.Ast;
using GoodNight.Service.Domain.Parse;
using GoodNight.Service.Domain.Write;
using Xunit;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick;
using Xunit.Sdk;

namespace GoodNight.Service.Domain.Test.Parse
{
  [FeatureFile("Parse/SceneParserTest.feature")]
  public class SceneParserTest : Xunit.Gherkin.Quick.Feature
  {
    private string? input;

    private ParseResult<IEnumerable<Content>>? result;

    private ITestOutputHelper output;

    public SceneParserTest(ITestOutputHelper output)
    {
      this.output = output;
    }


    [Given("the scene input")]
    public void ASceneNamed(DocString body)
    {
      input = body.Content;
    }

    [When("the parser parses the input")]
    public void TheParserParsesTheInput()
    {
      Assert.NotNull(input);
      result = new SceneParser().Parse(input!);
    }

    [Then(@"the scene has a name of ""(.*)""")]
    public void TheSceneHasANameOf(string name)
    {
      Assert.NotNull(result);
      Assert.True(result!.IsSuccessful);

      var contentName = result!.Result!
        .Single(c => c is Content.Name)
        as Content.Name;

      Assert.NotNull(contentName);
      Assert.Equal(name, contentName!.DisplayName);
    }

    [Then("the scene has only text content")]
    public void TheSceneHasOnlyTextContent()
    {
      Assert.NotNull(result);
      foreach(var content in result!.Result!)
      {
        Assert.IsType<Content.Text>(content);
      }
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

  }
}

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
  [FeatureFile("Parse/SceneParserTest.feature")]
  public class SceneParserTest : Xunit.Gherkin.Quick.Feature
  {
    private string? input;

    private ParseResult<IImmutableList<Content>>? result;

    private ITestOutputHelper output;

    public SceneParserTest(ITestOutputHelper output)
    {
      this.output = output;
    }

    private IEnumerable<Content> FindAllNodes(IEnumerable<Content> nodes)
    {
      foreach (var node in nodes)
      {
        yield return node;

        if (node is Content.Condition c)
        {
          foreach (var thenNode in FindAllNodes(c.Then))
          {
            yield return thenNode;
          }

          foreach (var elseNode in FindAllNodes(c.Else))
          {
            yield return elseNode;
          }
        }
      }
    }


    [Given("the scene input")]
    public void ASceneNamed(DocString body)
    {
      input = body.Content;
      output.WriteLine($"input is: --------8<--------");
      output.WriteLine(input);
      output.WriteLine("--------8<--------");
    }

    [When("the parser parses the input")]
    public void TheParserParsesTheInput()
    {
      Assert.NotNull(input);
      result = new SceneParser().Parse(input!);

      // to debug failing tests.
      output.WriteLine($"IsSuccessful: {result!.IsSuccessful}");
      output.WriteLine("Result:");
      if (result is not null && result.Result is not null)
      {
        foreach (var node in FindAllNodes(result.Result))
        {
          output.WriteLine($"  node: {node}");
          if (node is Content.Condition c) {
            output.WriteLine($"then: {c.Then.Count}, else: {c.Else.Count}");
          }
        }
      }

      output.WriteLine($"ErrorMessage: {result!.ErrorMessage}");
      output.WriteLine($"ErrorPosition: {result!.ErrorPosition}");
      output.WriteLine($"UnexpectedToken: {result!.UnexpectedToken}");
      output.WriteLine($"ExpectedToken: {result!.ExpectedToken}");
    }


    [Then(@"the parsed scene has a name of ""(.*)""")]
    public void TheParsedSceneHasANameOf(string name)
    {
      Assert.NotNull(result);
      Assert.True(result!.IsSuccessful);

      var contentName = result!.Result!
        .Single(c => c is Content.Name)
        as Content.Name;

      Assert.NotNull(contentName);
      Assert.Equal(name, contentName!.DisplayName);
    }

    [Then("the parsed scene has only text content")]
    public void TheParsedSceneHasOnlyTextContent()
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

    [Then("the parsed scene contains only one conditional")]
    public void TheParsedSceneContainsOnlyOneConditional()
    {
      ParsingSucceeds();
      Assert.NotNull(result!.Result);


      var element = result!.Result!.Single();
      Assert.IsType<Content.Condition>(element);
    }

    [Then(@"the parsed scene has at least one text node with ""(.*)""")]
    public void TheParsedSceneHasAtLeastOneTextNodeWith(string text)
    {
      ParsingSucceeds();
      Assert.NotNull(result!.Result);

      Assert.Contains(FindAllNodes(result!.Result!),
        n => n is Content.Text t ? t.Markdown == text : false);
    }
  }
}

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
        else if (node is Content.Option o)
        {
          foreach (var contentNode in FindAllNodes(o.Content))
          {
            yield return contentNode;
          }
        }
      }
    }

    private string? input;

    private ParseResult<IImmutableList<Content>>? result;



    [Given("the scene input")]
    public void ASceneNamed(DocString body)
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

    [Then(@"the result has (\d+) nodes?")]
    public void TheResultHasNNodes(int count)
    {
      Assert.Equal(count, result!.Result!.Count());
    }

    [Then(@"the scene has name ""(.*)""")]
    public void TheSceneHasNameString(string name)
    {
      Assert.Contains(result!.Result, node =>
        (node as Content.Name)!.DisplayName == name);
    }


    [Then(@"the node (\d+) is a ""(.*)""")]
    public void TheNodeNIsAType(int position, string type)
    {
      Assert.True(position <= result!.Result!.Count());
      Assert.Equal(type, result!.Result![position-1].GetType().Name);
    }

    [Then(@"the result has only ""(.*)"" nodes")]
    public void TheResultHasOnlyTypeNodes(string type)
    {
      Assert.All(result!.Result, node => {
        Assert.Equal(type, node.GetType().Name);
      });
    }


    private int CountNodesOfType(IEnumerable<Content> cs, string type)
    {
      int count = 0;
      foreach (var content in cs)
      {
        if (content.GetType().Name == type)
        {
          count += 1;
        }

        if (content is Content.Condition condition)
        {
          count += CountNodesOfType(condition.Then, type);
          count += CountNodesOfType(condition.Else, type);
        }
      }

      return count;
    }

    [Then(@"the result has (\d+) ""(.*)"" nodes in branches")]
    public void TheResultHasNumberTypeNodesInBranches(int count, string type)
    {
      Assert.Equal(count, CountNodesOfType(result!.Result!, type));
    }

    [Then(@"the node (\d+) has text ""(.*)""")]
    public void TheNodeNHasTextContent(int position, string content)
    {
      Assert.True(position <= result!.Result!.Count());
      var node = result!.Result![position-1];
      Assert.IsType<Content.Text>(node);
      var text = node as Content.Text;
      Assert.Equal(content, text!.Markdown);
    }

    [Then(@"the result has the tag ""(.*)""")]
    public void TheResultHasTagName(string tag)
    {
      Assert.Contains(result!.Result, node =>
        (node as Content.Tag)!.TagName == tag);
    }

    [Then(@"the result has the category ""(.*)""")]
    public void TheResultHasTheCategoryName(string name)
    {
      var category = result!.Result!
        .Single(n => n is Content.Category)
        as Content.Category;

      var expected = name == "quest/Hildas Hammer"
        ? new[] { "quest", "Hildas Hammer" }
        : name == "a/b/c/d/e/f/g/h/i/j/k/l/m/n"
        ? new[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l",
          "m", "n" }
        : name == "areas"
        ? new[] { "areas" }
        : new string[] { };

      Assert.Equal(expected, category!.Path);
    }
  }
}

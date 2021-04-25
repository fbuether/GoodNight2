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

    private ParseResult<Scene>? result;



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
      result = SceneParser.Parse(input!);

      // to debug failing tests.
      switch (result) {
        case ParseResult.Success<Scene> r:
          output.WriteLine($"Result is sucess.");
          output.WriteLine($"Value: {r.Result}.");

          output.WriteLine("Result:");
          foreach (var node in FindAllNodes(r.Result.Content))
          {
            output.WriteLine($"  node: {node}");
            if (node is Content.Condition c) {
              output.WriteLine($"then: {c.Then.Count}, else: {c.Else.Count}");
            }
          }

          break;
        case ParseResult.Failure<Scene> r:
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
      Assert.IsType<ParseResult.Failure<Scene>>(result);
    }

    [Then("parsing succeeds")]
    public void ParsingSucceeds()
    {
      Assert.NotNull(result);
      Assert.IsType<ParseResult.Success<Scene>>(result);
      Assert.NotNull((result as ParseResult.Success<Scene>)!.Result);
    }

    private Scene Get(ParseResult<Scene>? result)
    {
      Assert.NotNull(result);
      Assert.IsType<ParseResult.Success<Scene>>(result!);
 
      switch (result!) {
        case ParseResult.Success<Scene> r:
          return r.Result;
      }

      throw new Exception();
    }

    [Then(@"the result has (\d+) nodes?")]
    public void TheResultHasNNodes(int count)
    {
      Assert.Equal(count, Get(result).Content.Count());
    }

    [Then(@"the scene has name ""(.*)""")]
    public void TheSceneHasNameString(string name)
    {
      Assert.Contains(Get(result).Content, node =>
        (node as Content.Name)!.DisplayName == name);
    }


    [Then(@"the node (\d+) is a ""(.*)""")]
    public void TheNodeNIsAType(int position, string type)
    {
      Assert.True(position <= Get(result).Content.Count());
      Assert.Equal(type, Get(result).Content[position-1].GetType().Name);
    }

    [Then(@"the result has only ""(.*)"" nodes")]
    public void TheResultHasOnlyTypeNodes(string type)
    {
      Assert.All(Get(result).Content, node => {
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
      Assert.Equal(count, CountNodesOfType(Get(result).Content, type));
    }

    [Then(@"the node (\d+) has text ""(.*)""")]
    public void TheNodeNHasTextContent(int position, string content)
    {
      Assert.True(position <= Get(result).Content.Count());
      var node = Get(result).Content[position-1];
      Assert.IsType<Content.Text>(node);
      var text = node as Content.Text;
      Assert.Equal(content, text!.Value);
    }

    [Then(@"the result has the tag ""(.*)""")]
    public void TheResultHasTagName(string tag)
    {
      Assert.Contains(Get(result).Content, node =>
        (node as Content.Tag)!.TagName == tag);
    }

    [Then(@"the result has the category ""(.*)""")]
    public void TheResultHasTheCategoryName(string name)
    {
      var category = Get(result).Content
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

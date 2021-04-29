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

    private IEnumerable<Scene.Content> FindAllNodes(IEnumerable<Scene.Content> nodes)
    {
      foreach (var node in nodes)
      {
        yield return node;

        if (node is Scene.Content.Condition c)
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
        else if (node is Scene.Content.Option o)
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
          foreach (var node in FindAllNodes(r.Result.Contents))
          {
            output.WriteLine($"  node: {node}");
            if (node is Scene.Content.Condition c) {
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
      Assert.Equal(count, Get(result).Contents.Count());
    }

    [Then(@"the scene has name ""(.*)""")]
    public void TheSceneHasNameString(string name)
    {
      Assert.Contains(Get(result).Contents, node =>
        (node as Scene.Content.Name)!.DisplayName == name);
    }


    [Then(@"the node (\d+) is a ""(.*)""")]
    public void TheNodeNIsAType(int position, string type)
    {
      Assert.True(position <= Get(result).Contents.Count());
      Assert.Equal(type, Get(result).Contents[position-1].GetType().Name);
    }

    [Then(@"the result has only ""(.*)"" nodes")]
    public void TheResultHasOnlyTypeNodes(string type)
    {
      Assert.All(Get(result).Contents, node => {
        Assert.Equal(type, node.GetType().Name);
      });
    }


    private int CountNodesOfType(IEnumerable<Scene.Content> cs, string type)
    {
      int count = 0;
      foreach (var content in cs)
      {
        if (content.GetType().Name == type)
        {
          count += 1;
        }

        if (content is Scene.Content.Condition condition)
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
      Assert.Equal(count, CountNodesOfType(Get(result).Contents, type));
    }

    [Then(@"the node (\d+) has text ""(.*)""")]
    public void TheNodeNHasTextContent(int position, string content)
    {
      Assert.True(position <= Get(result).Contents.Count());
      var node = Get(result).Contents[position-1];
      Assert.IsType<Scene.Content.Text>(node);
      var text = node as Scene.Content.Text;
      Assert.Equal(content, text!.Value);
    }

    [Then(@"the result has the tag ""(.*)""")]
    public void TheResultHasTagName(string tag)
    {
      Assert.Contains(Get(result).Contents, node =>
        (node as Scene.Content.Tag)!.TagName == tag);
    }

    [Then(@"the result has the category ""(.*)""")]
    public void TheResultHasTheCategoryName(string name)
    {
      var category = Get(result).Contents.OfType<Scene.Content.Category>()
        .Single();

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

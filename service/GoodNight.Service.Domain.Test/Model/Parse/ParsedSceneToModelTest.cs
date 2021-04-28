using System;
using System.Collections.Generic;
using PContent = GoodNight.Service.Domain.Model.Parse.Content;
using PScene = GoodNight.Service.Domain.Model.Parse.Scene;
using WContent = GoodNight.Service.Domain.Model.Write.Content;
using WScene = GoodNight.Service.Domain.Model.Write.Scene;
using Xunit;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick;
using Gherkin.Ast;

namespace GoodNight.Service.Domain.Test.Model.Parse
{
  [FeatureFile("Model/Parse/ParsedSceneToModelTest.feature")]
  public class ParsedSceneToModelTest : Xunit.Gherkin.Quick.Feature
  {
    private ITestOutputHelper output;

    public ParsedSceneToModelTest(ITestOutputHelper output)
    {
      this.output = output;
    }

    private PScene? pscene;

    private WScene? wscene;

    [Given("the raw content")]
    public void TheRawContent(DocString body)
    {
      if (pscene == null)
      {
        pscene = PScene.Empty;
      }

      pscene = pscene with {Raw = body.Content};
    }

    [Given(@"the text content ""(.*)""")]
    public void TheTextContentString(string content)
    {
      if (pscene == null)
      {
        pscene = PScene.Empty;
      }

      pscene = pscene.AddContent(new PContent.Text(content));
    }

    [When("converting to a Write.Scene")]
    public void ConvertingToAWriteScene()
    {
      Assert.NotNull(pscene);

      wscene = pscene!.ToWriteModel();
      Assert.NotNull(wscene);
    }

    [Then(@"the Write.Scene has Name ""(.*)""")]
    public void TheWriteSceneHasNameString(string name)
    {
      Assert.Equal(name, wscene!.Name);
    }

    [Then(@"the Write.Scene is (not)? start")]
    public void TheWriteSceneIsMaybeStart(string not)
    {
      bool shouldBe = not != "not";
      Assert.Equal(shouldBe, wscene!.IsStart);
    }

    [Then(@"the Write.Scene is (not)? always shown")]
    public void TheWriteSceneIsMaybeAlwaysShown(string not)
    {
      bool shouldBe = not != "not";
      Assert.Equal(shouldBe, wscene!.ShowAlways);
    }

    [Then(@"the Write.Scene is (not)? forced to show")]
    public void TheWriteSceneIsMaybeForcedToShow(string not)
    {
      bool shouldBe = not != "not";
      Assert.Equal(shouldBe, wscene!.ForceShow);
    }

    [Then(@"the Write.Scene has no tags")]
    public void TheWriteSceneHasNoTags()
    {
      Assert.Empty(wscene!.Tags);
    }

    [Then(@"the Write.Scene has category ""(.*)""")]
    public void TheWriteSceneHasCategory(string cat)
    {
      Assert.Equal(cat, string.Join("/", wscene!.Category));
    }

    [Then(@"the Write.Scene has no sets")]
    public void TheWriteSceneHasNoSets()
    {
      Assert.Empty(wscene!.Sets);
    }

    [Then(@"the Write.Scene has no return")]
    public void TheWriteSceneHasNoReturn()
    {
      Assert.Null(wscene!.Return);
    }

    [Then(@"the Write.Scene has no continue")]
    public void TheWriteSceneHasNoContinue()
    {
      Assert.Null(wscene!.Continue);
    }

    [Then(@"the Write.Scene has no content")]
    public void TheWriteSceneHasNoContent()
    {
      Assert.Empty(wscene!.Contents);
    }

    [Then(@"the Write.Scene has (\d+) content nodes?")]
    public void TheWriteSceneHasNContentNodes(int number)
    {
      Assert.Equal(number, wscene!.Contents.Count);
    }

    [Then(@"content (\d+) of Write.Scene is text")]
    public void ContentNumberOfWriteSceneIsTextString(int number,
      DocString body)
    {
      Assert.True(wscene!.Contents.Count > number);
      var content = wscene!.Contents[number];

      Assert.IsType<WContent.Text>(content);
      var textContent = content as WContent.Text;

      Assert.Equal(body.Content, textContent!.Value);
    }
  }
}

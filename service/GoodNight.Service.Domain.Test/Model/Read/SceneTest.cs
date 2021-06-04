using System.Collections;
using System.Collections.Generic;
using Gherkin.Ast;
using NSubstitute;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick;
using Xunit;
using GoodNight.Service.Domain.Model.Read;
using System.Collections.Immutable;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Domain.Model.Expressions;

namespace GoodNight.Service.Domain.Test.Model.Read
{
  [FeatureFile("Model/Read/SceneTest.feature")]
  public class SceneTest : Xunit.Gherkin.Quick.Feature
  {
    private ITestOutputHelper output;

    public SceneTest(ITestOutputHelper output)
    {
      this.output = output;
    }


    private Scene scene = new Scene("", "", false, false, false,
      ImmutableList<Scene.Content>.Empty);

    private Player player = new Player("", 
      ImmutableList<(IReference<Quality>, Value)>.Empty);

    private Action? action = null;

    [Given(@"the scene content text ""(.*)""")]
    public void GivenTheSceneContentTextString(string text)
    {
      scene = scene with { Contents = scene.Contents.Add(
          new Scene.Content.Text(text)) };
    }

    [When(@"playing the scene")]
    public void WhenPlayingTheScene()
    {
      action = scene.Play(player);
      Assert.NotNull(action);
    }

    [Then(@"the action has no text")]
    public void ThenTheActionHasNoText()
    {
      Assert.Equal("", action!.Text);
    }

    [Then(@"the action has text ""(.*)""")]
    public void ThenTheActionHasTextString(string text)
    {
      Assert.Equal(text, action!.Text);
    }

    [Then(@"the action has text")]
    public void ThenTheActionHasTextBody(DocString body)
    {
      Assert.Equal(body.Content, action!.Text);
    }

    [Then(@"the action has no effects")]
    public void ThenTheActionHasNoEffects()
    {
      Assert.Equal(0, action!.Effects.Count);
    }

    [Then(@"the action has no options")]
    public void ThenTheActionHasNoOptions()
    {
      Assert.Equal(0, action!.Options.Count);
    }

    [Then(@"the action has no return")]
    public void ThenTheActionHasNoReturn()
    {
      Assert.Null(action!.Return);
    }

    [Then(@"the action has no continue")]
    public void ThenTheActionHasNoContinue()
    {
      Assert.Null(action!.Continue);
    }
  }
}

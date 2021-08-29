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
using GoodNight.Service.Domain.Parse;
using System.Linq;

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

    [Given(@"the option ""(.*)"" with requirement ""(.*)""")]
    public void GivenTheOptionStringWithRequirementString(string option,
      string requirement)
    {
      var req = ExpressionParser.Parse(requirement) as ParseResult.Success<Expression<string>>;
      Assert.NotNull(req);

      var reqq = req!.Result.Map(qn =>
        new Quality.Bool(qn, "story", null, "description", false, null) as IReference<Quality>);

      scene = scene with { Contents = scene.Contents.Add(
          new Scene.Content.Option(option, option, null,
            ImmutableList.Create(reqq),
            ImmutableList<(IReference<Quality>, Expression<IReference<Quality>>)>.Empty,
            new Scene("target", "", false, false, false,
              ImmutableList<Scene.Content>.Empty))) };
    }

    [Given(@"a player with ""(.*)""")]
    public void GivenAPlayerWithString(string qualityName)
    {
      player = player with { State = player.State.Add(
          (new Quality.Bool(qualityName, "story", null, "description", false, null),
            new Value.Bool(true))) };
    }

    [When(@"playing the scene")]
    public void WhenPlayingTheScene()
    {
      output.WriteLine($"scene: {scene}");
      output.WriteLine($"player: {player}");
      action = scene.Play(player, 14);
      Assert.NotNull(action);
      output.WriteLine($"action: {action}");
    }

    [Then(@"the action has an option that is (not )?available")]
    public void ThenTheActionHasAnOptionThatIsNotAvailable(string isNot)
    {
      var option = action!.Options.FirstOrDefault();
      Assert.NotNull(option);

      if (isNot.StartsWith("not")) {
        Assert.False(option!.IsAvailable);
      }
      else {
        Assert.True(option!.IsAvailable);
      }
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

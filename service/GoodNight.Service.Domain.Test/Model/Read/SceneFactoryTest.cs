using System;
using System.Collections;
using System.Collections.Generic;
using Gherkin.Ast;
using NSubstitute;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick;
using Xunit;
using GoodNight.Service.Domain.Model.Read;
using GoodNight.Service.Storage.Interface;
using PScene = GoodNight.Service.Domain.Model.Parse.Scene;
using GoodNight.Service.Domain.Util;
using GoodNight.Service.Domain.Parse;
using GoodNight.Service.Domain.Model.Expressions;

namespace GoodNight.Service.Domain.Test.Model.Read
{
  [FeatureFile("Model/Read/SceneFactoryTest.feature")]
  public class SceneFactoryTest : Xunit.Gherkin.Quick.Feature
  {
    private ITestOutputHelper output;

    public SceneFactoryTest(ITestOutputHelper output)
    {
      this.output = output;
    }


    private IRepository<Scene> scenes = Substitute.For<IRepository<Scene>>();

    private IRepository<Quality> qualities =
      Substitute.For<IRepository<Quality>>();

    private string story = "";

    private PScene parsed = PScene.Empty;

    private Result<Scene,string>? result;


    [Given(@"the story ""(.+)""")]
    public void GivenTheStoryString(string newStory)
    {
      story = newStory;
    }

    private void AddPContent(PScene.Content c)
    {
      parsed = parsed with { Contents = parsed.Contents.Add(c) };
    }

    private Expression<string> StringToExpression(string expr)
    {
      var result = ExpressionParser.Parse(expr);
      Assert.IsType<ParseResult.Success<Expression<string>>>(result);
      var presult = result as ParseResult.Success<Expression<string>>;
      Assert.NotNull(presult);
      return presult!.Result;
    }

    [Given(@"a name ""(.+)""")]
    public void GivenANameString(string name)
    {
      AddPContent(new PScene.Content.Name(name));
    }

    [Given(@"a set of ""(.+)"" to (.+)")]
    public void GivenASetOfStringToString(string quality, string expr)
    {
      var qualityKey = NameConverter.Concat(story, quality);
      var qualityRef = Substitute.For<IReference<Quality>>();
      qualityRef.Key.Returns(qualityKey);
      qualities.GetReference(qualityKey).Returns(qualityRef);

      output.WriteLine($"setup {qualityRef} to return {qualityRef.Key}");

      AddPContent(new PScene.Content.Set(quality,
          PScene.Content.SetOperator.Set,
          StringToExpression(expr)));
    }


    [When("building a scene")]
    public void WhenBuildingAScene()
    {
      result = SceneFactory.Build(scenes, qualities,
        parsed!, story);
    }


    [Then("building succeeds")]
    public void ThenBuildingSucceeds()
    {
      output.WriteLine("-------- building succeeds");
      output.WriteLine($"Result: {result}");
      Assert.NotNull(result);
      Assert.IsType<Result.Success<Scene,string>>(result);
    }

    [Then("building fails")]
    public void ThenBuildingFails()
    {
      output.WriteLine("-------- building fails");
      output.WriteLine($"Result: {result}");
      Assert.NotNull(result);
      Assert.IsType<Result.Failure<Scene,string>>(result);
    }

    private Scene Get()
    {
      Assert.NotNull(result);
      Assert.IsType<Result.Success<Scene,string>>(result);
      return (result as Result.Success<Scene,string>)!.Result;
    }

    private C GetContent<C>(int index)
      where C : class, Scene.Content
    {
      Assert.True(Get().Contents.Count >= index);
      var content = Get().Contents[index - 1];
      Assert.NotNull(content);

      Assert.IsType<Scene.Content.Effect>(content);
      var eff = (content as C)!;
      Assert.NotNull(eff);

      return eff;
    }

    [Then(@"the scene has story name ""(.+)""")]
    public void ThenTheSceneHasStoryNameString(string name)
    {
      Assert.Equal(name, Get().Story);
    }

    [Then(@"content (\d+) is an effect of ""(.+)"" set to (.+)")]
    public void ThenContentNumberIsAnEffectOfStringSetToString(
      int index, string quality, string expr)
    {
      var eff = GetContent<Scene.Content.Effect>(index);
      Assert.Equal(StringToExpression(expr).Map(qualities.GetReference),
        eff.Expression);

      qualities.Received().GetReference(NameConverter.Concat(story, quality));
      Assert.Equal(NameConverter.Concat(story, quality), eff.Quality.Key);
    }
  }
}

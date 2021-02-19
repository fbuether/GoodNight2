using GoodNight.Service.Domain.Model;
using GoodNight.Service.Domain.Model.Write;
using Xunit;
using Xunit.Gherkin.Quick;

namespace GoodNight.Service.Domain.Test.Write
{
  [FeatureFile("Play/StoryNames.feature")]
  public sealed class StoryNames : Feature
  {
    private Story? story;

    private string? urlname;

    [Given(@"a scenario named ""(.*)""")]
    public void CreateScenarioNamed(string name)
    {
      story = Story.Create(name);
    }

    [When(@"I generate the urlname")]
    public void GenerateTheUrlname()
    {
      urlname = story?.Urlname;
    }
  
    [Then(@"the urlname should be (.*)")]
    public void TheUrlnameShouldBe(string expected)
    {
      Assert.Equal(expected, this.urlname);
    }
  }
}

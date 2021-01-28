using Xunit;
using Xunit.Gherkin.Quick;
using GoodNight.Service.Domain.Play;

namespace GoodNight.Service.Domain.Test
{
  [FeatureFile("Play/StoryNames.feature")]
  public sealed class StoryNames : Feature
  {
    private Story? story;

    private string? urlname;

    [Given(@"a scenario named ""(.*)""")]
    public void CreateScenarioNamed(string name)
    {
      story = new Story(name);
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

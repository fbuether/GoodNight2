using Xunit.Gherkin.Quick;

namespace GoodNight.Service.Store.Test
{
  [FeatureFile("RepositoryTest.feature")]
  public class RepositoryTest : Feature
  {
    [Given("a new repository")]
    public void ANewRepository()
    {
    }

    [Then("the repository exists")]
    public void TheRepositoryExists()
    {
    }
  }
}


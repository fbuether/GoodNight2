using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Storage;
using Xunit.Gherkin.Quick;
using Xunit;

namespace GoodNight.Service.Storage.Test
{
  [FeatureFile("StoreTest.feature")]
  public class RepositoryTest : Feature
  {
    private IStore? store = null;

    private class Storable : IStorable<string>
    {
      private string key;
      
      public Storable(string key)
      {
        this.key = key;
      }

      public string GetKey()
      {
        return key;
      }
    }

    [Given("a store")]
    public void ANewRepository()
    {
      store = new Store();
    }

    [When(@"adding storable with key ""(.*)""")]
    public void AddingStorableWithKeyString(string key)
    {
      store!.Add<Storable, string>(new Storable(key));
    }

    [Then(@"fetching storable with key ""(.*)"" returns storable")]
    public void FetchingStorableWithKeyStringReturnsStorable(string key)
    {
      Assert.IsType<Storable>(store!.Get<Storable, string>(key));
    }

    [Then(@"fetching storable with key ""(.*)"" returns null")]
    public void FetchingStorableWithKeyStringReturnsNull(string key)
    {
      Assert.Null(store!.Get<Storable, string>(key));
    }
  }
}


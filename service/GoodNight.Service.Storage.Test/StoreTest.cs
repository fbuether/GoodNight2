using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Storage;
using Xunit.Gherkin.Quick;
using Xunit;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GoodNight.Service.Storage.Test
{
  [FeatureFile("StoreTest.feature")]
  public class RepositoryTest : Feature, IDisposable
  {
    private Stream journal = new MemoryStream();

    private IStore? store = null;

    public void Dispose()
    {
      journal.Dispose();
    }

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
    public void AStore()
    {
      journal.Dispose();
      journal = new MemoryStream();
      store = new Store(journal);
    }

    [Given(@"a store with journal ""(.*)""")]
    public void AStore(string initial)
    {
      journal.Dispose();
      journal = new MemoryStream();

      var writer = new StreamWriter(journal, Encoding.UTF8);
      writer.WriteLine(initial);
      writer.Flush();

      store = new Store(journal);
    }


    [When(@"adding storable with key ""(.*)""")]
    public async Task AddingStorableWithKeyString(string key)
    {
      await store!.Add<Storable, string>(new Storable(key));
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

    [Then("the journal is not empty")]
    public void TheJournalIsNotEmpty()
    {
      // journal.Flush();
      // journal.Seek(0, SeekOrigin.Begin);
      // var w = new StreamWriter(journal, Encoding.UTF8);
      // w.Write("newline!+");
      // w.Flush();

      // new StreamWriter(journal, Encoding.UTF8).WriteLine("newline.").;
      // journal.Flush();

      journal.Position = 0;

      Console.WriteLine("stream length " + journal.Length + ", " + journal.Position);

      var reader = new StreamReader(journal, Encoding.UTF8);

      Console.WriteLine("ppek " + reader.Peek());

      var content = reader.ReadToEnd();

      Console.WriteLine("----- journal content:" + content);
      Assert.True(false);

      Assert.NotEqual("", content);
    }
  }
}


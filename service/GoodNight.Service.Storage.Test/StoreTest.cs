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
    private IRepository<Demo, string>? repos = null;

    public void Dispose()
    {
      journal.Dispose();
    }

    private class Demo : IStorable<string>
    {
      private string key;

      public int Value { get; set; }
      
      public Demo(string key)
      {
        this.key = key;
      }

      public string GetKey()
      {
        return key;
      }
    }

    [Given("a repository for Demo")]
    public void ARepositoryForDemo()
    {
      journal.Dispose();
      journal = new MemoryStream();

      store = new Store();
      Assert.NotNull(store);

      repos = store.Create<Demo,string>(journal);
      Assert.NotNull(repos);
    }

    [When(@"adding Demo with key ""(.*)"" and value (\d+)")]
    public void AddingDemoWithKeyStringAndValueInt(string key, int value)
    {
      var demo = new Demo(key);
      demo.Value = value;
      repos!.Add(demo);
    }

    [Then(@"getting key ""(.*)"" returns null")]
    public void GettingKeyStringReturnsNull(string key)
    {
      var demo = repos!.Get(key);
      Assert.Null(demo);
    }

    [Then(@"getting key ""(.*)"" returns Demo with value (\d+)")]
    public void GettingKeyStringReturnsDemoWithValueInt(string key, int value)
    {
      var demo = repos!.Get(key);
      Assert.NotNull(demo);
      Assert.Equal(value, demo!.Value);
    }


    [Then(@"the journal is not empty")]
    public void TheJournalIsNotEmpty()
    {
      journal.Flush();
      journal.Seek(0, SeekOrigin.Begin);

      Console.WriteLine("stream length " + journal.Length + ", " + journal.Position);

      var reader = new StreamReader(journal, Encoding.UTF8);
      var content = reader.ReadToEnd();

      Assert.NotNull(content);
      Assert.NotEqual("", content);
    }
  }
}


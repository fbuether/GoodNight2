using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Storage;
using Xunit.Gherkin.Quick;
using Xunit;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Gherkin.Ast;

namespace GoodNight.Service.Storage.Test
{
  [FeatureFile("RepositoryTest.feature")]
  public class RepositoryTest : Xunit.Gherkin.Quick.Feature, IDisposable
  {
    private Stream journal = new MemoryStream();
    private string? journalContent = null;

    private IStore? store = null;
    private IRepository<Demo, string>? repos = null;

    public void Dispose()
    {
      journal.Dispose();
    }

    private record Demo(
      string Key,
      int Value)
      : IStorable<string>
    {
      public string GetKey()
      {
        return Key;
      }
    }

    [Given("a repository for Demo")]
    public void ARepositoryForDemo()
    {
      journal.Dispose();
      journal = new MemoryStream();
      journalContent = null;

      var newStore = new Store(journal);
      Assert.NotNull(newStore);
      store = newStore;

      repos = store.Create<Demo,string>("Demo");
      Assert.NotNull(repos);

      newStore.StartJournal();
    }

    [Given("a store with journal and repository for Demo")]
    public void AStoreWithJournalRepositoryForDemo(DocString body)
    {
      journal.Dispose();
      journal = new MemoryStream();
      journalContent = null;

      var writer = new StreamWriter(journal, Encoding.UTF8);
      writer.Write(body.Content);
      writer.Flush();
      journal.Seek(0, SeekOrigin.Begin);

      var newStore = new Store(journal);
      Assert.NotNull(newStore);
      store = newStore;

      repos = store.Create<Demo,string>("Demo");
      Assert.NotNull(repos);

      newStore.StartJournal();
    }

    [When(@"adding Demo with key ""(.*)"" and value (\d+)")]
    public void AddingDemoWithKeyStringAndValueInt(string key, int value)
    {
      repos!.Add(new Demo(key, value));
    }

    [When(@"updating Demo with key ""(.*)"" to value (\d+)")]
    public void UpdatingDemoWithKeyStringToValueInt(string key, int value)
    {
      repos!.Update(key, _ => new Demo(key, value));
    }

    [When(@"deleting Demo with key ""(.*)""")]
    public void DeletingDemoWithKeyString(string key)
    {
      repos!.Remove(key);
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



    private string GetJournal()
    {
      if (journalContent is null)
      {
        journal.Flush();
        if (store is not null)
        {
          ((IDisposable)store).Dispose();
        }

        journal.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(journal, Encoding.UTF8);
        journalContent = reader.ReadToEnd();
      }

      return journalContent;
    }

    [Then(@"the journal is not empty")]
    public void TheJournalIsNotEmpty()
    {
      var content = GetJournal();

      Assert.NotNull(content);
      Assert.NotEqual("", content);
    }

    [Then(@"the journal is this")]
    public void TheJournalIsThis(DocString body)
    {
      var content = GetJournal();
      Assert.Equal(body.Content, content);
    }
  }
}


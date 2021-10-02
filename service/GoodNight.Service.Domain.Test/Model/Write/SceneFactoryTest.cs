using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick;
using Gherkin.Ast;
using GoodNight.Service.Domain.Util;
using P = GoodNight.Service.Domain.Model.Parse;
using W = GoodNight.Service.Domain.Model.Write;
using System.IO;
using GoodNight.Service.Storage;
using System.Text.Json.Serialization;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Test.Model.Write
{
  [FeatureFile("Model/Write/SceneFactoryTest.feature")]
  public class SceneFactoryTest : Xunit.Gherkin.Quick.Feature
  {
    private ITestOutputHelper output;

    public SceneFactoryTest(ITestOutputHelper output)
    {
      this.output = output;
    }

    private P.Scene? pscene;
    private W.Scene? wscene;

    [Given("the raw content")]
    public void TheRawContent(DocString body)
    {
      if (pscene == null)
      {
        pscene = P.Scene.Empty;
      }

      pscene = pscene with {Raw = body.Content};
    }

    [Given(@"the text content ""(.*)""")]
    public void TheTextContentString(string content)
    {
      if (pscene == null)
      {
        pscene = P.Scene.Empty;
      }

      pscene = pscene with {Contents = pscene.Contents.Add(
          new P.Scene.Content.Text(content))};
    }

    [When("converting to a Write.Scene")]
    public void ConvertingToAWriteScene()
    {
      var journal = new MemoryStream();
      var store = new Store(new JsonConverter[] {}, journal) as IStore;

      Assert.NotNull(pscene);
      var parseResult = W.SceneFactory.Build(
        store.Create<Domain.Model.Write.Scene>(),
        store.Create<Domain.Model.Write.Quality>(),
        store.Create<Domain.Model.Read.Scene>(),
        pscene!, "story");

      Assert.IsType<Result.Success<W.Scene, string>>(parseResult);

      wscene = (parseResult as Result.Success<W.Scene, string>)?.Result;
      Assert.NotNull(wscene);
    }

    [Then(@"the Write.Scene has Name ""(.*)""")]
    public void TheWriteSceneHasNameString(string name)
    {
      Assert.Equal(name, wscene!.Name);
    }

    [Then(@"the Write.Scene has no tags")]
    public void TheWriteSceneHasNoTags()
    {
      Assert.Empty(wscene!.Tags);
    }

    [Then(@"the Write.Scene has category ""(.*)""")]
    public void TheWriteSceneHasCategory(string cat)
    {
      Assert.Equal(cat, string.Join("/", wscene!.Category));
    }
  }
}

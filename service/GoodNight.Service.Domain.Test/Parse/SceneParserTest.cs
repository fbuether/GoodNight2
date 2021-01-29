using Gherkin.Ast;
using GoodNight.Service.Domain.Parse;
using GoodNight.Service.Domain.Write;
using Xunit;
using Xunit.Gherkin.Quick;

namespace GoodNight.Service.Domain.Test.Parse
{
  [FeatureFile("Parse/SceneParserTest.feature")]
  public class SceneParserTest : Xunit.Gherkin.Quick.Feature
  {
    private Scene? scene;

    [Given(@"a scene with body")]
    public void ASceneNamed(DocString body)
    {
      this.scene = new SceneParser().Parse(body.Content);
    }

    [Then(@"the scene has a name of ""(.*)""")]
    public void TheSceneHasANameOf(string name)
    {
      Assert.NotNull(this.scene);
      Assert.NotEmpty(this.scene!.Content);

      Assert.Contains(this.scene!.Content, content => {

        return false;
        });
    }
  }
}

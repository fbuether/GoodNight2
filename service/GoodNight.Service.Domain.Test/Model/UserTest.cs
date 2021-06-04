using System;
using System.Collections;
using System.Collections.Generic;
using Gherkin.Ast;
using NSubstitute;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick;
using Xunit;

namespace GoodNight.Service.Domain.Test.Model
{
  [FeatureFile("Model/UserTest.feature")]
  public class UserTest : Xunit.Gherkin.Quick.Feature
  {
    private ITestOutputHelper output;

    public UserTest(ITestOutputHelper output)
    {
      this.output = output;
    }
  }
}

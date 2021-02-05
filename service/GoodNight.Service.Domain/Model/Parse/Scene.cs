using System;
using System.Collections.Immutable;
using ModelScene = GoodNight.Service.Domain.Model.Scene;

namespace GoodNight.Service.Domain.Model.Parse
{
  public record Scene(
    string Raw,
    IImmutableList<Content> Content)
  {
    public ModelScene ToModel()
    {
      var model = ModelScene.CreateDefault() with
        {
          Raw = Raw
        };

      foreach (var content in Content)
      {
        // todo!
      }

      return model;
    }
  }
}

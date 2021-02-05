using System;
using System.Collections.Immutable;

namespace GoodNight.Service.Domain.Model.Parse
{
  public record Scene(
    string Raw,
    IImmutableList<Content> Content)
  {
  }
}

using System;
using System.Collections.Immutable;

namespace GoodNight.Service.Domain.Write
{
  public record Scene (
    string Name,
    string Raw,
    IImmutableList<Content> Content)
  {
    public string Urlname
    {
      get
      {
        return NameConverter.OfString(Name);
      }
    }
  }
}

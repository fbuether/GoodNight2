using System;
using System.Collections.Immutable;

namespace GoodNight.Service.Domain.Write
{
  public record Scene (
    string Name,
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

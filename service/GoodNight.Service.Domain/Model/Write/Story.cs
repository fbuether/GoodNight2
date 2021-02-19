using System;
using System.Collections.Immutable;
using GoodNight.Service.Domain.Model.Parse;
using GoodNight.Service.Domain.Model;

namespace GoodNight.Service.Domain.Model.Write
{
  public record Story(
    string Name,
    IImmutableSet<Scene> Scenes,
    IImmutableSet<Quality> Qualities)
  {
    public string Urlname
    {
      get
      {
        return NameConverter.OfString(Name);
      }
    }


    // Create a new story with a given name.
    public static Story Create(string name)
    {
      return new Story(name,
        ImmutableHashSet<Scene>.Empty,
        ImmutableHashSet<Quality>.Empty);
    }
  }
}

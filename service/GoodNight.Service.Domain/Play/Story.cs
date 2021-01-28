using System;
using System.Collections.Immutable;

namespace GoodNight.Service.Domain.Play
{
  public record Story(
    string Name,
    IImmutableSet<Scene> Scenes)
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
      return new Story(name, ImmutableHashSet<Scene>.Empty);
    }
  }
}

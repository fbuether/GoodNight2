using System.Collections.Immutable;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Model.Write
{
  public record Scene(
    string Name,
    string Story, // urlname of story, for the key
    string Raw,
    IImmutableList<string> Tags,
    IImmutableList<string> Category,
    IImmutableList<string> OutLinks,
    IImmutableList<string> InLinks)
    : IStorable<Scene>
  {
    public string Urlname => NameConverter.OfString(Name);

    public string Key => NameConverter.Concat(Story, Urlname);
  }
}

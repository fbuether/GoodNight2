using System.Collections.Immutable;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Model.Write
{
  public record Scene(
    string Name,
    string Story, // urlname of story, for the key
    string Raw,

    // link to the read scene, for easier update
    IReference<Model.Read.Scene> ReadScene,

    IImmutableList<string> Tags,
    IImmutableList<string> Category,

    IImmutableList<IReference<Scene>> OutLinks,
    IImmutableList<IReference<Scene>> InLinks)
    : IStorable<Scene>
  {
    public string Urlname => NameConverter.OfString(Name);

    public string Key => NameConverter.Concat(Story, Urlname);
  }
}

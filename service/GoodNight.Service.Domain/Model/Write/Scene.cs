using System.Collections.Immutable;
using System.Linq;
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
    IImmutableList<IReference<Scene>> InLinks,
    IImmutableList<IReference<Quality>> Qualities)
    : IStorable<Scene>
  {
    public string Urlname => NameConverter.OfString(Name);

    public string Key => NameConverter.Concat(Story, Urlname);

    public Transfer.Scene ToTransfer()
    {
      return new Transfer.Scene(Name, Story, Raw,
        OutLinks.Select(l => new Transfer.Reference(l)),
        InLinks.Select(l => new Transfer.Reference(l)),
        Qualities.Select(l => new Transfer.Reference(l)));
    }
  }
}

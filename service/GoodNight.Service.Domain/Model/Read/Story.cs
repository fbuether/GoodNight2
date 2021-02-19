using System.Collections.Immutable;
using GoodNight.Service.Domain.Model.Write;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Read
{
  /// <summary>
  /// A Story is group of interlinked Scenes that a Player can play as an
  /// adventure. It comprises a set of Scenes as well as a set of Qualities.
  /// </summary>
  public record Story(
    string Name,
    string Urlname,

    string Description,
    bool Public,

    IImmutableSet<IStorableReference<Scene, string>> Scenes,
    IImmutableSet<IStorableReference<Quality, string>> Qualities)
    : IStorable<string>
  {
    public string GetKey()
    {
      return Urlname;
    }
  }
}

using System.Collections.Immutable;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Model.Read
{
  /// <summary>
  /// A Story is group of interlinked Scenes that a Player can play as an
  /// Adventure.
  ///
  /// Stories have a name and optionally an icon. They have a description and
  /// may be non-public. Finally, they contain sets of Scenes as well as
  /// Qualities, which together make up the actual story.
  /// </summary>
  public record Story(
    string Name,
    string? Icon,
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

    public string Urlname
    {
      get
      {
        return NameConverter.OfString(Name);
      }
    }

    public static Story Create(string name, string description = "") =>
      new Story(name, null, description, false,
        ImmutableHashSet<IStorableReference<Scene, string>>.Empty,
        ImmutableHashSet<IStorableReference<Quality, string>>.Empty);
  }
}

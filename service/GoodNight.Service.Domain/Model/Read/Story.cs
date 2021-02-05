using System.Collections.Immutable;

namespace GoodNight.Service.Domain.Read
{
  /// <summary>
  /// A Read.Story is one adventure that a user can play. It comprises a set
  /// of Scenes as well as a set of Qualities.
  /// </summary>
  public record Story(
    string Name,

    string Description,
    bool Public,

    IImmutableList<Scene> Scenes,
    IImmutableList<Quality> Qualities)
  {}
}

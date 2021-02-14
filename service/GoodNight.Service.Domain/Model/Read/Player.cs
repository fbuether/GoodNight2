using System.Collections.Immutable;

namespace GoodNight.Service.Domain.Read
{
  /// <summary>
  /// A Read.Player is a user playing a specific story. They have a name and a
  /// current state (a list of qualities along with the acquired values).
  /// </summary>
  public record Player(
    string User,
    string Name,
    IImmutableList<Property> State)
  {}
}

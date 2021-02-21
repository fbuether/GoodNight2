using System.Collections.Immutable;

namespace GoodNight.Service.Domain.Read
{
  /// <summary>
  /// A Player is a User playing a specific Story. They have a name and a
  /// current state, given as a list of qualities along with the acquired
  /// values.
  /// </summary>
  /// <remarks>
  /// Players are only persisted as part of an Adventure, which is part of a
  /// User.
  /// </remarks>
  public record Player(
    string Name,
    IImmutableSet<Property> State)
  {}
}

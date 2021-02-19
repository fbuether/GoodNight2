using System.Collections.Immutable;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Read
{
  /// <summary>
  /// A Player is a User playing a specific Story. They have a name and a
  /// current state, given as a list of qualities along with the acquired
  /// values.
  /// </summary>
  /// <remarks>
  /// This type belongs to the <see cref="User" /> root, and must not be stored
  /// individually.
  /// </remarks>
  public record Player(
    string Name,
    IStorableReference<Story, string> Story,
    IImmutableSet<Property> State)
  {}
}

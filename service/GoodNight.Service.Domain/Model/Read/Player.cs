
using System.Collections.Immutable;
using GoodNight.Service.Domain.Model.Expressions;

namespace GoodNight.Service.Domain.Read
{
  /// <summary>
  /// A Read.Player is a user playing a specific story. They have a name, a
  /// current state (a list of qualities along with the acquired values)
  /// and a history, a list of all scenes they have played.
  /// </summary>
  public record Player(
    string User,
    string Story,
    string Name,
    IImmutableList<Scene> History,
    IImmutableDictionary<string, Value> State)
  {}
}

using System.Linq;
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
    IImmutableList<Property> State)
  {
    public Player Apply(IImmutableList<Property> effects)
    {
      if (effects.Count == 0)
        return this;

      var newState = effects.Aggregate(State, (state, effect) =>
        ImmutableList.CreateRange(
          state.Select(prop => prop.Quality.Key == effect.Quality.Key
            ? effect
            : prop)));

      return this with { State = newState };
    }
  }
}

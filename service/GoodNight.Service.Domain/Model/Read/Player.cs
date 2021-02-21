using System.Linq;
using System.Collections.Immutable;
using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Domain.Model.Read.Error;

namespace GoodNight.Service.Domain.Model.Read
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
    IImmutableDictionary<IStorableReference<Quality, string>, Value> State)
  {
    public Player Apply(IImmutableList<Property> effects)
    {
      if (effects.Count == 0)
        return this;

      var newState = effects.Aggregate(State, (state, effect) =>
        state.SetItem(effect.Quality, effect.Value));

      return this with { State = newState };
    }

    public Value GetValueOf(IStorableReference<Quality, string> quality)
    {
      return State.GetValueOrDefault(quality)
        ?? quality.Get()?.GetDefault()
        ?? throw new InvalidQualityException($"Materialising Scene found " +
          $"invalid Quality {quality.Key}.");
    }
  }
}

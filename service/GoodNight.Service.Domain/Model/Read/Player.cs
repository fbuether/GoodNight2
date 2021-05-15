using System.Linq;
using System.Collections.Immutable;
using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Domain.Model.Read.Error;
using System;

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

    /// <summary>
    /// The state of this player. Associates each quality key with a state of
    /// the type of the quality.
    /// </summary>
    IImmutableDictionary<string, Value> State)
  {
    public Player Apply(IImmutableList<Property> effects)
    {
      if (effects.Count == 0)
        return this;

      var newState = effects.Aggregate(State, (state, effect) =>
        state.SetItem(effect.Quality.Key, effect.Value));

      return this with { State = newState };
    }

    public Value GetValueOf(IReference<Quality> qualityRef)
    {
      var quality = qualityRef.Get();
      if (quality is null)
        throw new InvalidQualityException($"Requested player state for "
          + "invalid Quality \"{qualityRef.Key}\".");

      return GetValueOf(quality);
    }

    public Value GetValueOf(Quality quality)
    {
      return State.GetValueOrDefault(quality.Key)
        ?? quality.GetDefault()
        ?? throw new InvalidQualityException($"Materialising Scene found " +
          $"invalid Quality {quality.Key}.");
    }


    internal Transfer.Player ToTransfer()
    {
      // todo: fixme: fix this.

      // var state = State.Select(qv => {
      //   var (qualityRef, value) = qv;
      //   var quality = qualityRef.Get();
      //   if (quality is null)
      //     throw new InvalidQualityException($"Player State contains invalid "
      //       + "Quality \"{qualityRef.Key}\".");

      //   return new Property(quality, valueString);
      // });

      return new Transfer.Player(Name,
        ImmutableList<Transfer.Property>.Empty);
    }
  }
}

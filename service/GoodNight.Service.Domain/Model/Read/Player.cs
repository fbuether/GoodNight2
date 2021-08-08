using System;
using System.Collections.Generic;
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

    /// <summary>
    /// The state of this player. Associates each quality key with a state of
    /// the type of the quality.
    /// </summary>
    IImmutableList<(IReference<Quality>, Value)> State)
  {
    public static Player Create(string name) =>
      new Player(name, ImmutableList<(IReference<Quality>,Value)>.Empty);

    public Player Apply(IImmutableList<Property> effects)
    {
      if (effects.Count == 0)
        return this;

      var state = State as IEnumerable<(IReference<Quality>,Value)>;
      foreach (var effect in effects) {
        state = state
          .Where(s => s.Item1.Key != effect.Quality.Key)
          .Append((effect.Quality, effect.Value));
      }

      return this with { State = ImmutableList.CreateRange(state) };
    }

    public Value GetValueOf(IReference<Quality> qualityRef)
    {
      var quality = qualityRef.Get();
      if (quality is null)
        throw new InvalidQualityException($"Requested player state for "
          + $"invalid Quality \"{qualityRef.Key}\".");

      return GetValueOf(quality);
    }

    public Value GetValueOf(Quality quality)
    {
      var binding = State.FirstOrDefault(qv => quality.Key == qv.Item1.Key);

      return binding.Item2
        ?? quality.GetDefault()
        ?? throw new InvalidQualityException($"Materialising Scene found "
          + $"invalid Quality {quality.Key}.");
    }


    internal Transfer.Player ToTransfer()
    {
      var state = State.Select(s => (s.Item1.Get(), s.Item2))
        .Where(s => s.Item1 is not null && !s.Item1.Hidden)
        .OfType<(Quality, Value)>()
        .Select(s => new Transfer.Property(
            s.Item1.ToTransfer(), s.Item1.Render(s.Item2)))
        .OrderBy(p => p.Quality.Name);

      return new Transfer.Player(Name, ImmutableList.CreateRange(state));
    }

    public override string ToString()
    {
      var state = string.Join(", ", State.Select(s => s.ToString()));
      return $"Player {{Name:{Name}, State:[{state}]}}";
    }
  }
}

using System;
using System.Collections.Immutable;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Read
{
  /// <summary>
  /// An Adventure is a path that a user has taken through a story. It includes
  /// the player they play as, the story they play on, the history of what they
  /// have experienced yet, and the materialised they currently are at.
  /// </summary>
  public record Adventure(
    Player Player,
    IStorableReference<Story, string> Story,
    IImmutableList<Log> History,
    Action Current)
  {
    /// <summary>
    /// Performs the next step in this Adventure given by the passed Option.
    /// This returns the new Adventure as well as a Consequence, which just
    /// encapsulates the newest history Log as well as the new Action.
    /// </summary>
    /// <param name="option">
    /// One of the options in Current.Options, or Current.Return or
    /// Current.Continue.
    /// <param>
    public (Adventure, Consequence) ContinueWith(Option option)
    {
      var scene = option.Scene.Get();
      var action = scene.Play(Player);

      throw new NotImplementedException();
    }
  }
}

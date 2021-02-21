using System;
using System.Collections.Immutable;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Model.Read
{
  /// <summary>
  /// An Adventure is a path that a user has taken through a story. It includes
  /// the player they play as, the story they play on, the history of what they
  /// have experienced yet, and the materialised they currently are at.
  /// </summary>
  /// <remarks>
  /// Adventures are only persisted as part of a user.
  /// </remarks>
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
    public (Adventure?, Consequence?) ContinueWith(string optionname)
    {
      var (log, nextScene) = Current.ContinueWith(optionname);
      if (log is null || nextScene is null)
        return (null, null);

      var scene = nextScene.Get();
      if (scene == null)
        return (null, null);

      var playerAfterChoice = Player.Apply(log.Effects);
      var action = scene.Play(nextScene, playerAfterChoice);
      var playerAfterScene = playerAfterChoice.Apply(action.Effects);

      var adventure = this with {
        History = History.Add(log),
        Current = action
      };

      return (adventure, new Consequence(log, action));
    }
  }
}

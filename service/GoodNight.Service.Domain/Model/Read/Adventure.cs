using System;
using System.Linq;
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
    string User,
    IReference<Story> Story,
    IImmutableList<IReference<Log>> History,
    Action Current)
    : IStorable<Adventure>
  {
    public string Key => NameConverter.Concat(User, Story.Key);

    /// <summary>
    /// Performs the next step in this Adventure given by the passed Option.
    /// This returns the new Adventure as well as a Consequence, which just
    /// encapsulates the newest history Log as well as the new Action.
    /// </summary>
    /// <param name="option">
    /// One of the options in Current.Options, or Current.Return or
    /// Current.Continue.
    /// <param>
    public (Adventure?, Consequence?) ContinueWith(IRepository<Log> logRepos,
      string optionname)
    {
      var lastNumber = History.LastOrDefault()?.Get()?.Number ?? 0;

      var (logRef, nextSceneRef) = Current.ContinueWith(logRepos, Player.Name,
        lastNumber, optionname);
      var log = logRef?.Get();
      var nextScene = nextSceneRef?.Get();
      if (logRef is null || log is null
        || nextSceneRef is null || nextScene is null)
        return (null, null);

      var playerAfterChoice = Player.Apply(log.Effects);
      var action = nextScene.Play(nextSceneRef, playerAfterChoice);
      var playerAfterScene = playerAfterChoice.Apply(action.Effects);

      var adventure = this with {
        History = History.Add(logRef),
        Current = action
      };

      return (adventure, new Consequence(log, action));
    }
  }
}

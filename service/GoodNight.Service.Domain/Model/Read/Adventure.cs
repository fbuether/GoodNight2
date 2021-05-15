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
  /// Adventures are referenced from the user, but persist seperately.
  /// </remarks>
  public record Adventure(
    Player Player,
    string User,
    IReference<Story> Story,

    /// <summary>
    /// A list of history items, which describe the path that a player has
    /// taken. Newest steps are at the end of the list.
    /// </summary>
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
    public (Adventure?, Consequence?) ContinueWith(string optionname)
    {
      var lastNumber = History.LastOrDefault()?.Get()?.Number ?? 0;

      var (log, nextScene) = Current.ContinueWith(Player.Name, lastNumber,
        optionname);
      if (log is null || nextScene is null)
        return (null, null);

      var playerAfterChoice = Player.Apply(log.Effects);
      var action = nextScene.Play(playerAfterChoice);
      var playerAfterScene = playerAfterChoice.Apply(action.Effects);

      var adventure = this with
        {
          History = History.Add(log),
          Current = action
        };

      return (adventure, new Consequence(log, action));
    }

    public Transfer.Adventure ToTransfer()
    {
      var history = History.Skip(History.Count - 5)
        .Select(h => h.Get())
        .OfType<Log>()
        .Select(h => h.ToTransfer());

      return new Transfer.Adventure(Player.ToTransfer(),
        ImmutableList.CreateRange(history),
        Current.ToTransfer());
    }
  }
}

using System;
using System.Linq;
using System.Collections.Immutable;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Domain.Model.Read.Transfer;
using GoodNight.Service.Domain.Model.Expressions;

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

    public static Adventure? Start(IReference<Story> story,
      IReference<User> user, string playerName)
    {
      Scene? firstScene = story.Get()?.Scenes.Select(s => s.Get())
        .FirstOrDefault(s => s is not null && s.IsStart);
      if (firstScene is null)
        return null;

      var player = new Player(playerName,
        ImmutableList<(IReference<Quality>,Value)>.Empty);
      var action = firstScene.Play(player);
      var affectedPlayer = player.Apply(action.Effects);

      return new Adventure(affectedPlayer, user.Key, story,
        ImmutableList<IReference<Log>>.Empty, action);
    }

    /// <summary>
    /// Performs the next step in this Adventure given by the passed Option.
    /// This returns the new Adventure as well as a Consequence, which just
    /// encapsulates the newest history Log as well as the new Action.
    /// </summary>
    /// <param name="option">
    /// One of the options in Current.Options, or Current.Return or
    /// Current.Continue.
    /// <param>
    public Adventure? ContinueWith(string optionname)
    {
      var lastNumber = History.LastOrDefault()?.Get()?.Number ?? 0;

      var (log, nextScene) = Current.ContinueWith(User, lastNumber,
        optionname);
      if (log is null || nextScene is null)
        return null;

      var playerAfterChoice = log.Chosen is Choice.Action ca
        ? Player.Apply(ca.Effects)
        : Player;
      var action = nextScene.Play(playerAfterChoice);
      var playerAfterScene = playerAfterChoice.Apply(action.Effects);

      return this with
        {
          Player = playerAfterScene,
          History = History.Add(log),
          Current = action
        };
    }

    public Transfer.Adventure ToTransfer(bool fullHistory)
    {
      var histTrail = fullHistory
        ? History.Skip(History.Count - 5)
        : ImmutableList.Create(History.Last());

      var history = histTrail
        .Select(h => h.Get())
        .OfType<Log>()
        .Select(h => h.ToTransfer());

      return new Transfer.Adventure(Player.ToTransfer(),
        ImmutableList.CreateRange(history),
        Current.ToTransfer());
    }

    public override string ToString()
    {
      return $"Adventure {{Player:{Player}, User:{User}, Story:{Story}, "
        + "History: [" + string.Join(", ", History.Select(h => h.ToString()))
        + $"], Current:{Current}}}";
    }
  }
}

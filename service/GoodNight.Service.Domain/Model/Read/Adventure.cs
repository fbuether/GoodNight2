using System;
using System.Linq;
using System.Collections.Immutable;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Domain.Model.Read.Error;
using System.Threading;

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

    /// <summary>
    /// The seed used for any randomness in the current scene. Storing this
    /// here ensures that the player will get the same Action on reloading their
    /// story.
    /// </summary>
    int RndSeed,

    /// <summary>
    /// The Scene that this player last entered.
    /// It is stored un-realised here, in order to accomodate future changes
    /// to the scene. When playing the story, the Scene is realised into an
    /// action.
    /// </summary>
    IReference<Scene> Current)

    : IStorable<Adventure>
  {
    static private ThreadLocal<Random> rndGen = new ThreadLocal<Random>(() =>
      new Random());

    private static int GetNextRandom() => (rndGen.Value ?? new Random()).Next();

    public string Key => NameConverter.Concat(User, Story.Key);

    public static Adventure? Start(IReference<Story> story,
      IReference<User> user, string playerName)
    {
      Scene? firstScene = story.Get()?.Scenes.Select(s => s.Get())
        .FirstOrDefault(s => s is not null && s.IsStart);
      if (firstScene is null)
        return null;

      return new Adventure(Player.Create(playerName), user.Key, story,
        ImmutableList<IReference<Log>>.Empty, GetNextRandom(), firstScene);
    }

    /// <summary>
    /// Performs the next step in this Adventure given by the passed Option.
    /// This returns the new Adventure with the played out scene as the newest
    /// log entry and the player updated, as well as a new seed and current
    /// scene.
    /// </summary>
    /// <param name="option">
    /// One of the options in Current.Options, or "return" or
    /// "continue".
    /// <param>
    public Adventure? ContinueWith(string optionname)
    {
      var lastNumber = History.LastOrDefault()?.Get()?.Number ?? 0;

      var asAction = Current.Get()?.Play(Player, RndSeed);
      if (asAction is null) {
        throw new InvalidSceneException(
          $"Player \"{Player.Name}\" is at invalid Scene \"{Current.Key}\"");
      }

      var playerAfterAction = Player.Apply(asAction.Effects);
      var (log, nextScene) = asAction.ContinueWith(User, lastNumber,
        optionname);
      if (log is null || nextScene is null)
        return null;

      var playerAfterScene = log.Chosen is Choice.Action ca
        ? playerAfterAction.Apply(ca.Effects)
        : playerAfterAction;

      return this with
      {
        Player = playerAfterScene,
        History = History.Add(log),
        RndSeed = GetNextRandom(),
        Current = nextScene
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

      var scene = Current.Get();
      if (scene is null) {
        throw new InvalidSceneException(
          $"Player \"{Player.Name}\" plays invalid Scene \"{Current.Key}\".");
      }

      var action = scene.Play(Player, RndSeed);
      return new Transfer.Adventure(
        Player.Apply(action.Effects).ToTransfer(),
        ImmutableList.CreateRange(history),
        action.ToTransfer());
    }

    public override string ToString()
    {
      return $"Adventure {{Player:{Player}, User:{User}, Story:{Story}, "
        + "History: [" + string.Join(", ", History.Select(h => h.ToString()))
        + $"], Current:{Current}}}";
    }
  }
}

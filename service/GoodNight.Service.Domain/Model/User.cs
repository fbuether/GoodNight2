using GoodNight.Service.Storage.Interface;
using System.Collections.Immutable;
using System.Linq;
using System;
using GoodNight.Service.Domain.Model.Read;
using GoodNight.Service.Domain.Util;
using GoodNight.Service.Domain.Model.Expressions;

namespace GoodNight.Service.Domain.Model
{
  /// <summary>
  /// A user is one person registered at GoodNight.
  /// They may have multiple players, at most one per story.
  /// </summary>
  /// <remarks>
  /// This type is a group root and can be stored directly.
  /// </remarks>
  public record User(
    Guid Guid,
    string Name,
    string EMail,

    IImmutableSet<IReference<Adventure>> Adventures)
    : IStorable<User>
  {
    public string Key => Guid.ToString();

    public (User, Adventure)? ContinueAdventure(Story story,
      string optionname)
    {
      var adventure = this.Adventures
        .First(a => a.Get()?.Story.Key == story.Key)?
        .Get();
      if (adventure is null)
        return null;

      var newAdventure = adventure.ContinueWith(optionname);
      if (newAdventure is null)
        return null;

      return (AddAdventure(newAdventure), newAdventure);
    }

    public User RemoveAdventure(IReference<Adventure> adventure)
    {
      return this with {Adventures = Adventures.Remove(adventure)};
    }

    public User AddAdventure(Adventure adventure)
    {
      var filteredAdventures = ImmutableHashSet.CreateRange(
        this.Adventures
        .Where(a => a.Key != adventure.Key));

      return this with {Adventures = filteredAdventures.Add(adventure)};
    }

    public Adventure? GetAdventure(string storyUrlname)
    {
      var advKey = NameConverter.Concat(Key, storyUrlname);
      return Adventures.FirstOrDefault(adv => adv.Key == advKey)?.Get();
    }

    public Result<(User,Adventure),string> StartAdventure(
      IReference<Story> story, string playerName)
    {
      var existingAdventure = GetAdventure(story.Key);
      if (existingAdventure is not null)
        return new Result.Failure<(User,Adventure),string>(
          "User has already started an adventure in this story.");

      Scene? firstScene = story.Get()?.Scenes.Select(s => s.Get())
        .FirstOrDefault(s => s is not null && s.IsStart);
      if (firstScene is null)
        return new Result.Failure<(User,Adventure),string>(
          "The story has no first scene to start at.");

      var player = new Player(playerName,
        ImmutableDictionary<string,Value>.Empty);
      var adventure = new Adventure(player, Key, story,
        ImmutableList<IReference<Log>>.Empty,
        firstScene.Play(player));
      var user = AddAdventure(adventure);

      return new Result.Success<(User,Adventure),string>((user, adventure));
    }
  }
}

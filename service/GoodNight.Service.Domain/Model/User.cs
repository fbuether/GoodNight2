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
    string Id,
    string Name,
    string EMail,

    IImmutableSet<IReference<Adventure>> Adventures)
    : IStorable<User>
  {
    public string Key => Id;

    public static User Create(string id) => new User(id, "", "",
      ImmutableHashSet<IReference<Adventure>>.Empty);

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

      var adventure = Adventure.Start(story, this, playerName);
      if (adventure is null)
        return new Result.Failure<(User,Adventure),string>(
          "The story has no first scene to start at.");

      var user = AddAdventure(adventure);
      return new Result.Success<(User,Adventure),string>((user, adventure));
    }

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


    public override string ToString()
    {
      return $"User {{Id:{Id}, Name:{Name}, EMail:{EMail}, "
        + "Adventures:[" +
        string.Join(", ", Adventures.Select(a => a.ToString())) + "]}";
    }
  }
}

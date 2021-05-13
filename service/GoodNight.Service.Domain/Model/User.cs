using GoodNight.Service.Storage.Interface;
using System.Collections.Immutable;
using System.Linq;
using System;
using GoodNight.Service.Domain.Model.Read;

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

    public (User, Consequence)? ContinueAdventure(Story story,
      string optionname)
    {
      var adventure = this.Adventures
        .First(a => a.Get()?.Story.Key == story.Key)?
        .Get();
      if (adventure is null)
        return null;

      var (newAdventure, consequence) = adventure.ContinueWith(optionname);
      if (newAdventure is null || consequence is null)
        return null;

      return (AddAdventure(newAdventure), consequence);
    }

    public User AddAdventure(Adventure adventure)
    {
      var filteredAdventures = ImmutableHashSet.CreateRange(
        this.Adventures
        .Where(a => a.Key != adventure.Key));

      return this with {Adventures = filteredAdventures.Add(adventure)};
    }
  }
}

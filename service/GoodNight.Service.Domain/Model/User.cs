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
    string Guid,
    string Name,
    string EMail,

    IImmutableSet<IReference<Adventure>> Adventures)
    : IStorable<User>
  {
    public string Key => Guid;

    public (User, Consequence)? ContinueAdventure(
      IRepository<Adventure> adventureRepos, IRepository<Log> logRepos,
      Story story, string optionname)
    {
      var adventureRef = this.Adventures
        .First(a => a.Get()?.Story.Key == story.Key);
      var adventure = adventureRef?.Get();
      if (adventure is null || adventureRef is null)
        return null;

      var (newAdventure, consequence) = adventure.ContinueWith(logRepos,
        optionname);
      if (newAdventure is null || consequence is null)
        return null;

      var newAdventureRef = adventureRepos.Add(newAdventure);
      if (newAdventureRef is null)
        return null;

      var newSelf = this with {
        Adventures = this.Adventures
          .Remove(adventureRef)
          .Add(newAdventureRef)
      };

      return (newSelf, consequence);
    }
  }
}

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
    string Name,
    string EMail,

    IImmutableSet<Adventure> Adventures)
    : IStorable
  {
    public string GetKey()
    {
      return this.EMail;
    }

    public (User, Consequence)? ContinueAdventure(Story story,
      string optionname)
    {
      var adventure = this.Adventures.First(a => a.Story.Key == story.GetKey());
      if (adventure is null)
        return null;

      var (newAdventure, consequence) = adventure.ContinueWith(optionname);
      if (newAdventure is null || consequence is null)
        return null;

      var newSelf = this with {
        Adventures = this.Adventures
          .Remove(adventure)
          .Add(newAdventure)
      };

      return (newSelf, consequence);
    }
  }
}

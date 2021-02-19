using GoodNight.Service.Domain.Read;
using GoodNight.Service.Storage.Interface;
using System.Collections.Immutable;
using System.Linq;
using System;

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
    : IStorable<string>
  {
    public string GetKey()
    {
      return this.EMail;
    }

    public (User, Consequence)? ContinueAdventure(string storyname,
      string optionname)
    {
      var adventure = this.Adventures.First(a => a.Story.Key == storyname);
      if (adventure == null)
        return null;

      var option = adventure.Current.Options
        .First(o => o.Scene.Key == optionname);
      if (option == null)
        return null;

      var (newAdventure, consequence) = adventure.ContinueWith(option);

      var newSelf = this with {
        Adventures = this.Adventures
          .Remove(adventure)
          .Add(newAdventure)
      };

      return (newSelf, consequence);
    }
  }
}

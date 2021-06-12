using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Model.Read
{
  /// <summary>
  /// A Story is group of interlinked Scenes that a Player can play as an
  /// Adventure.
  ///
  /// Stories have a name and optionally an icon. They have a description and
  /// may be non-public. Finally, they contain sets of Scenes as well as
  /// Qualities, which together make up the actual story.
  /// </summary>
  public record Story(
    string Name,
    string? Icon,
    string Description,
    bool Public,

    IImmutableSet<IReference<Scene>> Scenes,
    IImmutableSet<IReference<Quality>> Qualities)
    : IStorable<Story>
  {
    public string Key => Urlname;

    public string Urlname => NameConverter.OfString(Name);

    public static Story Create(string name, string description = "") =>
      new Story(name, null, description, true,
        ImmutableHashSet<IReference<Scene>>.Empty,
        ImmutableHashSet<IReference<Quality>>.Empty);


    public virtual bool Equals(Story? other)
    {
      if (other is null || this.Key != other.Key)
        return false;

      if (this.Icon != other.Icon ||
        this.Description != other.Description ||
        this.Public != other.Public)
        return false;

      if (Scenes.Count != other.Scenes.Count)
        return false;

      if (Qualities.Count != other.Qualities.Count)
        return false;

      if (Scenes.Zip(other.Scenes).Any(ab => ab.Item1 != ab.Item2))
        return false;

      if (Qualities.Zip(other.Qualities).Any(ab => ab.Item1 != ab.Item2))
        return false;

      return true;
    }

    public override int GetHashCode() => this.Key.GetHashCode();


    public Story AddScene(Scene scene)
    {
      var newScene = scene with {Story = Urlname};
      var oldScene = Scenes.FirstOrDefault(s => s.Key == newScene.Key);
      var removed = oldScene is null ? Scenes : Scenes.Remove(oldScene);
      return this with {Scenes = removed.Add(newScene)};
    }

    public Story AddQuality(Quality quality)
    {
      var newQuality = quality with {Story = Urlname};
      var oldQuality = Qualities.FirstOrDefault(s => s.Key == newQuality.Key);
      var removed = oldQuality is null
        ? Qualities
        : Qualities.Remove(oldQuality);
      return this with {Qualities = removed.Add(newQuality)};
    }

    public Transfer.Story ToTransfer()
    {
      return new Transfer.Story(Name, Urlname, Icon, Description);
    }
  }
}

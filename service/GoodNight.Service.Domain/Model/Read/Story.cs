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
      return RemoveScene(scene.Key) with {Scenes = Scenes.Add(newScene)};
    }

    public Story RemoveScene(string sceneKey)
    {
      var el = Scenes.FirstOrDefault(s => s.Key == sceneKey);
      return el is null ? this : (this with {Scenes = Scenes.Remove(el)});
    }


    public Story AddQuality(Quality quality)
    {
      var newQuality = quality with {Story = Urlname};
      var removed = RemoveQuality(quality.Key);
      return removed with {Qualities = Qualities.Add(newQuality)};
    }

    public Story RemoveQuality(string qualityKey)
    {
      var el = Qualities.FirstOrDefault(q => q.Key == qualityKey);
      return el is null ? this : this with {Qualities = Qualities.Remove(el)};
    }


    public Transfer.Story ToTransfer()
    {
      return new Transfer.Story(Name, Urlname, Icon, Description);
    }

    public override string ToString()
    {
      return $"Story {{Name:{Name}, Icon:{Icon}, Description:{Description}, "
        + $"Public:{Public}, Scenes: ["
        + string.Join(", ", Scenes.Select(s => s.ToString()))
        + "], Qualities: ["
        +  string.Join(", ", Qualities.Select(q => q.ToString()))
        + "]}";
    }
  }
}

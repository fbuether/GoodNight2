using System;
using System.Linq;
using System.Collections.Immutable;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Model.Write
{
  /// <summary>
  /// A StoryHeader is a Story reduced to its properties required for long
  /// lists of Stories, e.g. Story selection.
  /// </summary>
  public record StoryHeader(
    string Name,
    string Urlname,
    string? Icon,
    string Description);


  public record Story(
    string Name,
    // string? Icon,
    // string Description,
    IReference<User> Creator,
    IImmutableSet<IReference<Scene>> Scenes,
    IImmutableSet<IReference<Quality>> Qualities)
    : IStorable<Story>
  {
    public string Urlname => NameConverter.OfString(Name);

    public string Key => Urlname;

    /// <summary>
    /// Create a new story with a given name.
    /// </summary>
    public static Story Create(string name, IReference<User> user)
    {
      return new Story(name, user,
        ImmutableHashSet<IReference<Scene>>.Empty,
        ImmutableHashSet<IReference<Quality>>.Empty);
    }

    public StoryHeader ToHeader()
    {
      return new StoryHeader(Name, Urlname, null, "");
    }


    public Category GetContentAsCategories()
    {
      var category = this.Scenes.Aggregate(Category.Empty,
        (cat, sceneRef) => {
          var scene = sceneRef.Get();
          return scene is not null
            ? cat.AddScene(scene, scene.Category)
            : cat;
        });

      category = this.Qualities.Aggregate(category,
        (cat, qualityRef) => {
          var quality = qualityRef.Get();
          return quality is not null
            ? cat.AddQuality(quality, quality.Category)
            : cat;
        });

      return category.Sorted();
    }


    public Scene? GetScene(string sceneUrlname)
    {
      var name = NameConverter.Concat(Urlname, sceneUrlname);
      return Scenes.FirstOrDefault(s => s.Key == name)?.Get();
    }

    public (Story, Scene) AddScene(Scene scene)
    {
      var newScene = scene with {Story = Urlname};
      var oldElement = Scenes.FirstOrDefault(s => s.Key == newScene.Key);
      var withoutOld = oldElement is null ? Scenes : Scenes.Remove(oldElement);
      return (this with {Scenes = withoutOld.Add(newScene)}, newScene);
    }


    public Quality? GetQuality(
      string qualityUrlname)
    {
      var name = NameConverter.Concat(Urlname, qualityUrlname);
      return Qualities.FirstOrDefault(q => q.Key == name)?.Get();
    }

    public (Story, Quality) AddQuality(Quality quality)
    {
      var newQl = quality with {Story = Urlname};
      var oldElement = Qualities.FirstOrDefault(q => q.Key == newQl.Key);
      var withoutOld = oldElement is null
        ? Qualities
        : Qualities.Remove(oldElement);
      return (this with {Qualities = withoutOld.Add(newQl)}, newQl);
    }
  }
}

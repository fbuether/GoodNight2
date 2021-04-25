using System.Linq;
using System;
using System.Collections.Immutable;
using GoodNight.Service.Domain.Model.Parse;
using GoodNight.Service.Domain.Model;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Domain.Parse;
using GoodNight.Service.Domain.Util;
using System.Collections.Generic;
using ReadStory = GoodNight.Service.Domain.Model.Read.Story;

namespace GoodNight.Service.Domain.Model.Write
{
  /// <summary>
  /// A StoryHeader is a Story reduced to its properties required for long
  /// lists of Stories, e.g. Story selection.
  /// </summary>
  public record StoryHeader(
    string name,
    string urlname,
    string description);


  public record Category(
    string name,
    IImmutableList<Category> categories,
    IImmutableList<Scene> scenes,
    IImmutableList<Quality> qualities)
  {
    public static Category Empty = new Category("",
      ImmutableList<Category>.Empty,
      ImmutableList<Scene>.Empty,
      ImmutableList<Quality>.Empty);

    public static Category OfName(string name) => Category.Empty with
      {
        name = name
      };
  }


  public record Story(
    string Name,
    IImmutableSet<IReference<Scene>> Scenes,
    IImmutableSet<IReference<Quality>> Qualities)
    : IStorable
  {
    public string Urlname
    {
      get
      {
        return NameConverter.OfString(Name);
      }
    }

    public string GetKey()
    {
      return Urlname;
    }

    /// <summary>
    /// Create a new story with a given name.
    /// </summary>
    public static Story Create(string name)
    {
      return new Story(name,
        ImmutableHashSet<IReference<Scene>>.Empty,
        ImmutableHashSet<IReference<Quality>>.Empty);
    }

    public StoryHeader ToHeader()
    {
      return new StoryHeader(Name, Urlname, "-");
    }


    private Category AddSceneToCategories(Category root,
      Scene scene, IEnumerable<string> path)
    {
      if (!path.Any())
      {
        return root with { scenes = root.scenes.Add(scene) };
      }

      var first = path.First();
      var cat = root.categories.FirstOrDefault(c => c.name == first);

      var newCategories = cat is not null
        ? root.categories.Remove(cat)
        : root.categories;
      var subCategory = cat is not null
        ? cat
        : Category.OfName(first);

      return root with { categories = newCategories.Add(
          AddSceneToCategories(subCategory, scene, path.Skip(1))) };
    }

    private Category AddQualityToCategories(Category root, Quality quality,
      IEnumerable<string> path)
    {
      if (!path.Any())
      {
        return root with { qualities = root.qualities.Add(quality) };
      }

      var first = path.First();
      var cat = root.categories.FirstOrDefault(c => c.name == first);

      var newCategories = cat is not null
        ? root.categories.Remove(cat)
        : root.categories;
      var subCategory = cat is not null
        ? cat
        : Category.OfName(first);

      return root with { categories = newCategories.Add(
          AddQualityToCategories(subCategory, quality, path.Skip(1))) };
    }

    public Category GetContentAsCategories()
    {
      var category = this.Scenes.Aggregate(Category.Empty,
        (cat, sceneRef) => {
          var scene = sceneRef.Get();
          return scene is not null
            ? AddSceneToCategories(cat, scene, scene.Category)
            : cat;
        });

      category = this.Qualities.Aggregate(category,
        (cat, qualityRef) => {
          var quality = qualityRef.Get();
          return quality is not null
            ? AddQualityToCategories(cat, quality, quality.Category)
            : cat;
        });

      return category;
    }


    public IReference<Scene>? GetScene(string sceneUrlname)
    {
      var name = NameConverter.Concat(Urlname, sceneUrlname);
      return Scenes.FirstOrDefault(s => s.Key == name);
    }

    /// <summary>
    /// Adopts this scene to be a scene of this story.
    /// Technically sets the field Story, so that the key of the scene can
    /// be generated properly; must be done prior to adding to the store.
    /// </summary>
    public Scene SeizeScene(Scene scene)
    {
      return scene with {Story = Urlname};
    }

    public (Story, IReference<Scene>) InsertNewScene(
      IReference<Scene> scene)
    {
      return (this with {Scenes = Scenes.Add(scene)}, scene);
    }

    public (Story, IReference<Scene>) ReplaceScene(
      IReference<Scene> scene)
    {
      var oldElement = Scenes.FirstOrDefault(s => s.Key == scene.Key);
      var withoutOld = oldElement is null ? Scenes : Scenes.Remove(oldElement);
      return (this with {Scenes = withoutOld.Add(scene)}, scene);
    }


    public IReference<Quality>? GetQuality(
      string qualityUrlname)
    {
      var name = NameConverter.Concat(Urlname, qualityUrlname);
      return Qualities.FirstOrDefault(q => q.Key == name);
    }

    public Quality SeizeQuality(Quality quality)
    {
      return quality with {Story = Urlname};
    }

    public (Story, IReference<Quality>) InsertNewQuality(
      IReference<Quality> quality)
    {
      return (this with {Qualities = Qualities.Add(quality)}, quality);
    }

    public (Story, IReference<Quality>) ReplaceQuality(
      IReference<Quality> quality)
    {
      return (this with {Qualities = Qualities.Add(quality)}, quality);
    }


    public ReadStory? ToReadStory()
    {
      return ReadStory.Create(Name);
    }
  }
}

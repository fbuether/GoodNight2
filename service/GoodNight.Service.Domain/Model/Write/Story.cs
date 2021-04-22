using System.Linq;
using System;
using System.Collections.Immutable;
using GoodNight.Service.Domain.Model.Parse;
using GoodNight.Service.Domain.Model;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Domain.Parse;
using GoodNight.Service.Domain.Util;
using System.Collections.Generic;

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
    IImmutableSet<Scene> Scenes,
    IImmutableSet<Quality> Qualities)
    : IStorable<string>
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
        ImmutableHashSet<Scene>.Empty,
        ImmutableHashSet<Quality>.Empty);
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
        (cat, scene) => AddSceneToCategories(cat, scene, scene.Category));

      category = this.Qualities.Aggregate(category,
        (cat, quality) => AddQualityToCategories(
          cat, quality, quality.Category));

      return category;
    }


    private Result<Scene, string> ParseScene(string raw)
    {
      return SceneParser.Parse(raw).ToResult().Map(s => s.ToModel());
    }

    public Result<(Story, Scene), string> AddNewScene(string raw)
    {
      return ParseScene(raw).Bind<(Story, Scene)>(scene =>
      {
        if (this.Scenes.Any(s => s.Urlname == scene.Urlname))
        {
          return new Result.Failure<(Story, Scene), string>(
            "A scene of that name already exists.");
        }
        else
        {
          return new Result.Success<(Story, Scene), string>(
            (InsertScene(scene), scene));
        }
      });
    }

    public Result<(Story, Scene), string> EditScene(Scene oldScene, string raw)
    {
      return ParseScene(raw)
        .Map(scene => (ReplaceScene(oldScene, scene), scene));
    }

    public Story InsertScene(Scene scene)
    {
      return this with { Scenes = this.Scenes.Add(scene) };
    }

    public Story ReplaceScene(Scene oldScene, Scene newScene)
    {
      return this with { Scenes = this.Scenes.Remove(oldScene).Add(newScene) };
    }


    private Result<Quality, string> ParseQuality(string raw)
    {
      return QualityParser.Parse(raw).ToResult();
    }

    public Result<(Story, Quality), string> AddNewQuality(string raw)
    {
      return ParseQuality(raw).Bind<(Story, Quality)>(quality =>
      {
        if (this.Qualities.Any(q => q.Urlname == quality.Urlname))
        {
          return new Result.Failure<(Story, Quality), string>(
            "A quality of that name already exists.");
        }
        else {
          return new Result.Success<(Story, Quality), string>(
            (this with { Qualities = this.Qualities.Add(quality) },
              quality));
        }
      });
    }

    public Result<(Story, Quality), string> EditQuality(Quality oldQuality,
      string raw)
    {
      return ParseQuality(raw).Map(quality => (
          this with {
            Qualities = this.Qualities.Remove(oldQuality).Add(quality)
          },
          quality));
    }
  }
}

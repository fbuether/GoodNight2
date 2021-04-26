using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace GoodNight.Service.Domain.Model.Write
{
  public record Category(
    string Name,
    IImmutableList<Category> Categories,
    IImmutableList<Scene> Scenes,
    IImmutableList<Quality> Qualities)
  {
    internal static Category Empty = new Category("",
      ImmutableList<Category>.Empty,
      ImmutableList<Scene>.Empty,
      ImmutableList<Quality>.Empty);

    internal static Category OfName(string name) => Category.Empty with
      {
        Name = name
      };


    internal Category AddQuality(Quality quality, IEnumerable<string> path)
    {
      if (!path.Any())
      {
        return this with { Qualities = Qualities.Add(quality) };
      }

      var first = path.First();

      var cat = Categories.FirstOrDefault(c => c.Name == first);
      var cleanedCategories = cat is not null
        ? Categories.Remove(cat)
        : Categories;
      var subCategory = cat is not null
        ? cat
        : Category.OfName(first);

      var newCategories = cleanedCategories.Add(
        subCategory.AddQuality(quality, path.Skip(1)));

      return this with { Categories = newCategories };
    }

    internal Category AddScene(Scene scene, IEnumerable<string> path)
    {
      if (!path.Any())
      {
        return this with { Scenes = Scenes.Add(scene) };
      }

      var first = path.First();
      var cat = Categories.FirstOrDefault(c => c.Name == first);
      var cleanedCategories = cat is not null
        ? Categories.Remove(cat)
        : Categories;
      var subCategory = cat is not null
        ? cat
        : Category.OfName(first);

      var newCategories = cleanedCategories.Add(
        subCategory.AddScene(scene, path.Skip(1)));

      return this with { Categories = newCategories };
    }

  }
}

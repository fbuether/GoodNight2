using System.Collections.Immutable;
using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Domain.Parse;
using GoodNight.Service.Domain.Util;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Model.Write
{
  public abstract record Quality(
    string Name,
    string Story, // urlname of the story, for the key

    Type Type,
    string Raw,
    bool Hidden,

    IImmutableList<string> Tags,
    IImmutableList<string> Category,
    string? Scene,

    string Description)
    : IStorable<Quality>
  {
    public string Urlname
    {
      get
      {
        return NameConverter.OfString(Name);
      }
    }

    public string Key => NameConverter.Concat(Story, Urlname);

    public record Bool(
      string Name,
      string Story, // urlname of the story, for the key
      string Raw,
      bool Hidden,
      IImmutableList<string> Tags,
      IImmutableList<string> Category,
      string? Scene,
      string Description)
      : Quality(Name, Story, Type.Bool, Raw, Hidden, Tags, Category, Scene,
        Description) {}

    public record Int(
      string Name,
      string Story, // urlname of the story, for the key
      string Raw,
      bool Hidden,
      IImmutableList<string> Tags,
      IImmutableList<string> Category,
      string? Scene,
      string Description,

      int? Minimum,
      int? Maximum)
      : Quality(Name, Story, Type.Int, Raw, Hidden, Tags, Category, Scene,
        Description) {}

    public record Enum(
      string Name,
      string Story, // urlname of the story, for the key
      string Raw,
      bool Hidden,
      IImmutableList<string> Tags,
      IImmutableList<string> Category,
      string? Scene,
      string Description,

      IImmutableDictionary<int, string> Levels)
      : Quality(Name, Story, Type.Enum, Raw, Hidden, Tags, Category, Scene,
        Description) {}


    public static Result<Quality, string> Parse(string raw)
    {
      return QualityParser.Parse(raw).ToResult();
    }
  }
}

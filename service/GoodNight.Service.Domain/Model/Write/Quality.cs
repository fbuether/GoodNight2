using System.Collections.Immutable;
using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Model.Write
{
  public abstract record Quality(
    string Name,
    Type Type,
    string Raw,
    bool Hidden,
    string? Scene,
    string Description)
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

    public record Bool(
      string Name,
      string Raw,
      bool Hidden,
      string? Scene,
      string Description)
      : Quality(Name, Type.Bool, Raw, Hidden, Scene, Description) {}

    public record Int(
      string Name,
      string Raw,
      bool Hidden,
      string? Scene,
      string Description,
      int? Minimum,
      int? Maximum)
      : Quality(Name, Type.Int, Raw, Hidden, Scene, Description) {}

    public record Enum(
      string Name,
      string Raw,
      bool Hidden,
      string? Scene,
      string Description,
      IImmutableDictionary<int, string> Levels)
      : Quality(Name, Type.Enum, Raw, Hidden, Scene, Description) {}
    }
}

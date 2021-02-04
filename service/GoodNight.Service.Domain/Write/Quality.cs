using System.Collections.Immutable;
using GoodNight.Service.Domain.Write.Expressions;

namespace GoodNight.Service.Domain.Write
{
  public abstract record Quality(
    string Name,
    Type Type,
    string Raw,
    bool Hidden,
    string? Scene,
    string Description)
  {
    public string Urlname
    {
      get
      {
        return NameConverter.OfString(Name);
      }
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
      int Minimum,
      int Maximum)
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

using GoodNight.Service.Domain.Model.Expressions;

namespace GoodNight.Service.Domain.Read
{
  /// <summary>
  /// A Read.Quality describes anything that describes a specific player.
  /// It has a name and type, as well as optionally a scene that it can trigger.
  /// </summary>
  public abstract record Quality(
    string Name,
    Type Type,
    string? Icon,
    string Description,
    string? Scene)
  {
    public record Bool(
      string Name,
      string? Icon,
      string Description,
      string? Scene)
      : Quality(Name, Type.Bool, Icon, Description, Scene) {}

    public record Int(
      string Name,
      string? Icon,
      string Description,
      string? Scene)
      : Quality(Name, Type.Int, Icon, Description, Scene) {}

    public record Enum(
      string Name,
      string? Icon,
      string Description,
      string? Scene)
      : Quality(Name, Type.Enum, Icon, Description, Scene) {}
  }
}

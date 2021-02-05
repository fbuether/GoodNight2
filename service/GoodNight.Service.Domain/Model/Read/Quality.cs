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
    string Description,
    string? Scene)
  {
    
  }
}

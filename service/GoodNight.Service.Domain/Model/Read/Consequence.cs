
namespace GoodNight.Service.Domain.Read
{
  /// <summary>
  /// A Consequence is the result of executing a choice. It describes the
  /// choice itself through the Log, which folds the previous scene into
  /// the taken choice, and attaches the next Scene which unfolds from it as it
  /// applies to the player in the shape of an Action.
  /// This is only used as the return type of the `/do` endpoint, to ease
  /// typing it.
  /// </summary>
  public record Consequence(
    Log Log,
    Action Action)
  {}
}

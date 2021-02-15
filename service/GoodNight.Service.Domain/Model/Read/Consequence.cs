
namespace GoodNight.Service.Domain.Read
{
  /// <summary>
  /// A Consequence is the result of executing a choice. It describes the
  /// choice itself through the Action, which folds the previous scene into
  /// the taken choice, and attaches the next Scene which unfolds from it.
  /// </summary>
  public record Consequence(
    Action Action,
    Scene Scene)
  {}
}

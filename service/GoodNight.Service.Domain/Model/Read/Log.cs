using System.Collections.Immutable;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Read
{
  /// <summary>
  /// A Choice is an Option that the player has already taken. It is part of
  /// a history Log entry.
  /// </summary>
  public abstract record Choice
  {
    public record Action(
      IStorableReference<Scene, string>? Scene,
      string Text,
      IImmutableList<Property> Effects)
      : Choice {}

    public record Return(
      IStorableReference<Scene, string>? Scene)
      : Choice {}

    public record Continue(
      IStorableReference<Scene, string>? Scene)
      : Choice {}
  }

  /// <summary>
  /// A Log is the protocol of an Action when one of its Options has been taken.
  /// It documents the Action including its text and effects, as well as the
  /// Option that the player chose.
  /// </summary>
  /// <remarks>
  /// Logs are only persisted as part of an Adventure, which is part of a User.
  /// </remarks>
  public record Log(
    IStorableReference<Scene, string> Scene,
    string Text,
    IImmutableList<Property> Effects,
    Choice Chosen)
  {}
}

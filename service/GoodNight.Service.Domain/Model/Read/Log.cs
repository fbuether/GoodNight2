using System.Collections.Immutable;

namespace GoodNight.Service.Domain.Read
{
  /// <summary>
  /// A Choice is an Option that the player has already taken. It is part of
  /// a history Log entry.
  /// </summary>
  public abstract record Choice
  {
    public record Action(
      string Scene,
      string Text,
      IImmutableList<Property> Effects)
      : Choice {}

    public record Return(string Scene)
      : Choice {}

    public record Continue(string Scene)
      : Choice {}
  }

  /// <summary>
  /// A Log is the protocol of an Action when one of its Options has been taken.
  /// It documents the Action including its text and effects, as well as the
  /// Option that the player chose.
  /// </summary>
  public record Log(
    string Urlname,
    string Text,
    IImmutableList<Property> Effects,
    Choice Chosen)
  {}
}

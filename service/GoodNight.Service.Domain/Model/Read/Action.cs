using System.Collections.Immutable;
using GoodNight.Service.Domain.Model.Expressions;

namespace GoodNight.Service.Domain.Read
{
  public abstract record Choice
  {
    public record Option(
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
  /// A Read.SceneLog is the result of chosing an option. It folds a scene into
  /// the path taken, removing other options, and adding the effects the choice
  /// had.
  /// </summary>
  public record Action(
    string Urlname,
    string Text,
    IImmutableList<Property> Effects,
    Choice Chosen
  ) {}
}

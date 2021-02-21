using System.Collections.Immutable;
using GoodNight.Service.Domain.Model.Expressions;

namespace GoodNight.Service.Domain.Model
{
  /// <summary>
  /// Content is a part of a Scene. Each Scene contains a set of Content units
  /// that specify the text, options, requirements and conditions of this
  /// Scene.
  /// </summary>
  /// <typeparam name="TScene">
  /// The type of references to Scenes.
  /// </typeparam>
  /// <typeparam name="TQuality">
  /// The type of references to Qualities.
  /// </typeparam>
  public record Content<TScene,TQuality>
  {
    public record Text<S,Q>(
      string Value)
      : Content<S,Q> {}

    public record Require<S,Q>(
      Expression<Q> Expression)
      : Content<S,Q> {}

    public record Option<S,Q>(
      S Scene,
      IImmutableList<Content<S,Q>> Content)
      : Content<S,Q> {}

    public record Condition<S,Q>(
      Expression<Q> If,
      IImmutableList<Content<S,Q>> Then,
      IImmutableList<Content<S,Q>> Else)
      : Content<S,Q> {}

    public record Include<S,Q>(
      Q Scene)
      : Content<S,Q> {}
  }
}

using System.Collections.Immutable;
using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Read
{
  using StoredQuality = IStorableReference<Quality, string>;
  using Expression = Expression<IStorableReference<Quality, string>>;

  public abstract record Content
  {
    public record Text(
      string Value)
      : Content {}

    public record Effect(
      StoredQuality Quality,
      Expression Expression)
      : Content {}

    /// <summary>
    /// Adds an Option the Player may take here. It has a specific Urlname to
    /// make this Option uniquely chooseable, a descriptive text and optionally
    // an icon. A set of requirement Expressions may guard this Option. If
    /// chosen, it may have a set of Effects and a Scene to continue to.
    /// </summary>
    public record Option(
      string Urlname,
      string Description,
      string? Icon,
      IImmutableList<Expression> Requirements,
      IImmutableList<(StoredQuality, Expression)> Effects,
      IStorableReference<Scene, string> Scene)
      : Content {}

    /// <summary>
    /// Returns to a previous Scene. This may only occur once on each
    /// manifestation of a Scene.
    /// </summary>
    public record Return(
      IStorableReference<Scene, string> Scene)
      : Content {}

    /// <summary>
    /// Continues to a next Scene. This may only occur once on each
    /// manifestation of a Scene.
    /// </summary>
    public record Continue(
      IStorableReference<Scene, string> Scene)
      : Content {}

    /// <summary>
    /// Adds Content if a specific condition holds or does not hold.
    /// One of the branches may be an empty list if no Content exists.
    /// </summary>
    public record Condition(
      Expression If,
      IImmutableList<Content> Then,
      IImmutableList<Content> Else)
      : Content {}

    /// <summary>
    /// Includes the content of another Scene.
    /// This inserts the Content of the other Scene at exactly this position,
    /// considering e.g. a Condition.
    /// It does *not* transitively include Includes in the other Scene's
    /// Content.
    /// </summary>
    public record Include(
      IStorableReference<Scene, string> Scene)
      : Content {}
  }

  /// <summary>
  /// A Scene is one unit of a Story. It represents one step in the Adventure
  /// that a player may take. It is not materialised and contains the
  /// information to adopt itself to a specific Player.
  /// </summary>
  public record Scene(
    string Name,
    IImmutableList<Content> Content)
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
  }
}


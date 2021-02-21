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

    public record Option(
      string Urlname,
      string Description,
      string? Icon,
      IImmutableList<Expression> Requirements,
      IImmutableList<(StoredQuality, Expression)> Effects,
      IStorableReference<Scene, string> Scene)
      : Content {}

    public record Return(
      IStorableReference<Scene, string> Scene)
      : Content {}

    public record Continue(
      IStorableReference<Scene, string> Scene)
      : Content {}

    public record Condition(
      Expression If,
      IImmutableList<Content> Then,
      IImmutableList<Content> Else)
      : Content {}

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


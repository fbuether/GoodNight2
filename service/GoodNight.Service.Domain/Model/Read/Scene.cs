using System.Collections.Immutable;
using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Read
{
  using Expression = Expression<IStorableReference<Quality, string>>;


  public abstract record OptionContent
  {
    public record Text(
      string Value)
      : OptionContent {}

    public record Require(
      Expression Expression)
      : OptionContent {}

    public record Condition(
      Expression If,
      IImmutableList<OptionContent> Then,
      IImmutableList<OptionContent> Else)
      : OptionContent {}
  }

  public abstract record SceneContent
  {
    public record Text(
      string Value)
      : SceneContent {}

    public record Effect(
      IStorableReference<Quality, string> Quality,
      Expression Expression)
      : SceneContent {}

    public record Option(
      IStorableReference<Scene, string> Scene,
      IImmutableList<OptionContent> Content)
      : SceneContent {}

    public record Return(
      IStorableReference<Scene, string> Scene)
      : SceneContent {}

    public record Continue(
      IStorableReference<Scene, string> Scene)
      : SceneContent {}

    public record Condition(
      Expression If,
      IImmutableList<SceneContent> Then,
      IImmutableList<SceneContent> Else)
      : SceneContent {}

    public record Include(
      IStorableReference<Scene, string> Scene)
      : SceneContent {}
  }

  /// <summary>
  /// A Scene is one unit of a Story. It represents one step in the Adventure
  /// that a player may take. It is not materialised and contains the
  /// information to adopt itself to a specific Player.
  /// </summary>
  public record Scene(
    string Name,
    IImmutableList<SceneContent> Content)
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


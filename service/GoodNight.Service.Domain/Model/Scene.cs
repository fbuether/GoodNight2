using System;
using System.Collections.Immutable;
using GoodNight.Service.Domain.Model.Expressions;

namespace GoodNight.Service.Domain.Model
{
  public record Content()
  {
    public record Text(
      string Value)
      : Content() {}

    public record Require(
      Expression Expression)
      : Content() {}

    public record Option(
      string Scene,
      IImmutableList<Content> Content)
      : Content() {}

    public record Condition(
      Expression If,
      IImmutableList<Content> Then,
      IImmutableList<Content> Else)
      : Content() {}

    public record Include(
      string Scene)
      : Content() {}
  }

  public record Scene(
    string Name,
    string Raw,
    bool IsStart,
    bool ShowAlways,
    bool ForceShow,
    IImmutableList<string> Tags,
    IImmutableList<string> Category,

    IImmutableList<Tuple<string, Expression>> Sets,

    string? Return,
    string? Continue,

    IImmutableList<Content> Content)
  {
    public string Urlname
    {
      get
      {
        return NameConverter.OfString(Name);
      }
    }

    public static Scene CreateDefault()
    {
      return new Scene("", "", false, false, false,
        ImmutableList<string>.Empty,
        ImmutableList<string>.Empty,
        ImmutableList<Tuple<string, Expression>>.Empty,
        null,
        null,
        ImmutableList<Content>.Empty);
    }
  }
}

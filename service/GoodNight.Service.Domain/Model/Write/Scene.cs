using System;
using System.Collections.Immutable;
using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Model.Write
{
  /// <summary>
  /// Content is a part of a Scene. Each Scene contains a set of Content units
  /// that specify the text, options, requirements and conditions of this
  /// Scene.
  /// </summary>
  public record Content
  {
    public record Text(
      string Value)
      : Content {}

    public record Require(
      Expression<string> Expression)
      : Content {}

    public record Option(
      string Scene,
      IImmutableList<Content> Content)
      : Content {}

    public record Condition(
      Expression<string> If,
      IImmutableList<Content> Then,
      IImmutableList<Content> Else)
      : Content {}

    public record Include(
      string Scene)
      : Content {}
  }


  public record Scene(
    string Name,
    string Raw,
    bool IsStart,
    bool ShowAlways,
    bool ForceShow,
    IImmutableList<string> Tags,
    IImmutableList<string> Category,

    IImmutableList<(string, Expression<string>)> Sets,

    string? Return,
    string? Continue,

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

    public static Scene CreateDefault()
    {
      return new Scene("", "", false, false, false,
        ImmutableList<string>.Empty,
        ImmutableList<string>.Empty,
        ImmutableList<(string, Expression<string>)>.Empty,
        null,
        null,
        ImmutableList<Content>.Empty);
    }

    public Scene AddContent(Content newContent)
    {
      return this with { Content = Content.Add(newContent) };
    }
  }
}

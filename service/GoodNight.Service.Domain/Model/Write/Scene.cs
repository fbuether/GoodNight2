using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Domain.Parse;
using GoodNight.Service.Domain.Util;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Model.Write
{
  /// <summary>
  /// Content is a part of a Scene. Each Scene contains a set of Content units
  /// that specify the text, options, requirements and conditions of this
  /// Scene.
  /// </summary>
  public abstract record Content
  {
    public record Text(
      string Value)
      : Content;

    /// <summary>
    /// Requirements pose expressions that must hold true for an option to be
    /// available.
    /// Consequently, they may only occur inside an Option.
    /// </summary>
    public record Require(
      Expression<string> Expression)
      : Content {}

    public record Option(
      string Scene,
      IImmutableList<Content> Contents)
      : Content
    {
    }

    public record Condition(
      Expression<string> If,
      IImmutableList<Content> Then,
      IImmutableList<Content> Else)
      : Content;

    public record Include(
      string Scene)
      : Content;
  }


  public record Scene(
    string Name,
    string Story, // urlname of story, for the key
    string Raw,
    bool IsStart,
    bool ShowAlways,
    bool ForceShow,
    IImmutableList<string> Tags,
    IImmutableList<string> Category,

    IImmutableList<(string, Expression<string>)> Sets,

    string? Return,
    string? Continue,

    IImmutableList<Content> Contents)
    : IStorable<Scene>
  {
    public string Urlname => NameConverter.OfString(Name);

    public string Key => NameConverter.Concat(Story, Urlname);

    public static Scene Empty
    {
      get
      {
        return new Scene("", "", "", false, false, false,
          ImmutableList<string>.Empty,
          ImmutableList<string>.Empty,
          ImmutableList<(string, Expression<string>)>.Empty,
          null,
          null,
          ImmutableList<Content>.Empty);
      }
    }

    public Scene AddContent(Content newContent)
    {
      return this with { Contents = Contents.Add(newContent) };
    }
  }
}

using System;
using System.Linq;
using System.Collections.Immutable;
using GoodNight.Service.Domain.Model.Expressions;
using System.Collections.Generic;

namespace GoodNight.Service.Domain.Model.Parse
{
  public record Scene(
    string Raw,
    IImmutableList<Scene.Content> Contents)
  {
    public interface Content
    {
      // actual text for the player to read. This can be markdown.
      public record Text(
        string Value)
        : Content;

      /// <summary>
      /// The name of this scene. Must occur on each scene.
      /// </summary>
      public record Name(
        string DisplayName)
        : Content;

      // this scene is the start of the story
      // must not be within a Condition.
      public record IsStart
        : Content;

      // always show this action, even if not available
      // must not be within a Condition.
      public record ShowAlways
        : Content;

      // should this action automatically be taken, if possible?
      // must not be within a Condition.
      // should usually also have a Require so the scene does not instantly
      // trigger for every player.
      public record ForceShow
        : Content;


      // to classify scenes for writing add categories or tags
      // tags are opaque strings
      // several tags may be attached to each scene
      public record Tag(
        string TagName)
        : Content;

      // categories behave like a path, similar to a directory
      // each scene may have at most one category
      public record Category(
        IImmutableList<string> Path)
        : Content;

      public enum SetOperator
      {
        Set, Add, Sub, Mult, Div
      }

      // set a quality of the player to a new value
      // only used on the top level of an action/option
      public record Set(
        string Quality,
        SetOperator Operator,
        Expression<string> Expression)
        : Content;

      // require the player to fulfil an expression to take this scene
      // only used inside of options, or on the top level
      public record Require(
        Expression<string> Expression)
        : Content;


      // something the player can do here, contains a body of description
      public record Option(
        string Scene,
        IImmutableList<Content> Content)
        : Content;

      // allow the player to return to another scene without requirements
      public record Return(
        string Scene)
        : Content;

      // this scene can continue on into another scene, without requirements
      public record Continue(
        string Scene)
        : Content;


      // consider a set of settings only if a condition holds
      public record Condition(
        Expression<string> If,
        IImmutableList<Content> Then,
        IImmutableList<Content> Else)
        : Content;

      // includes another scene here, completely, except for its own includes
      public record Include(
        string Scene)
        : Content;
    }

    public static Scene Empty => new Scene("", ImmutableList.Create<Content>());


    public IEnumerable<Content> GetFlatContents()
    {
      return GetFlatContents(Contents);
    }

    private IEnumerable<Content> GetFlatContents(IEnumerable<Content> contents)
    {
      foreach (var content in contents)
      {
        yield return content;

        switch (content)
        {
          case Content.Option o:
            foreach (var subContent in GetFlatContents(o.Content))
            {
              yield return subContent;
            }
            break;

          case Content.Condition c:
            foreach (var subContent in GetFlatContents(c.Then))
            {
              yield return subContent;
            }
            foreach (var subContent in GetFlatContents(c.Else))
            {
              yield return subContent;
            }
            break;
        }
      }
    }

    public IEnumerable<string> GetOutLinks()
    {
      foreach (var content in GetFlatContents())
      {
        switch (content)
        {
          case Content.Option c:
            yield return c.Scene;
            break;

          case Content.Return c:
            yield return c.Scene;
            break;

          case Content.Continue c:
            yield return c.Scene;
            break;

          case Content.Include c:
            yield return c.Scene;
            break;
        }
      }
    }
  }
}

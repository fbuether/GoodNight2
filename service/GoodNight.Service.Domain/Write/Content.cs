using System.Collections.Immutable;
using GoodNight.Service.Domain.Write.Expressions;

namespace GoodNight.Service.Domain.Write
{
  public record Content()
  {
    // actual text for the player to read. This can be markdown.
    public record Text(
      string Markdown)
      : Content() {}

    // The name of this scene. Must occur on each scene.
    public record Name(
      string DisplayName)
      : Content() {}

    // this scene is the start of the story
    // must not be within a Condition.
    public record IsStart()
      : Content() {}

    // always show this action, even if not available
    // must not be within a Condition.
    public record ShowAlways()
      : Content() {}

    // should this action automatically be taken, if possible?
    // must not be within a Condition.
    // should usually also have a Require so the scene does not instantly
    // trigger for every player.
    public record ForceShow()
      : Content() {}


    // to classify scenes for writing add categories or tags
    // tags are opaque strings
    // several tags may be attached to each scene
    public record Tag(
      string TagName)
      : Content() {}

    // categories behave like a path, similar to a directory
    // each scene may have at most one category
    public record Category(
      IImmutableList<string> Path)
      : Content() {}


    // set a quality of the player to a new value
    // usually only used in Actions
    public record Set(
      string Quality,
      Expression Expression)
      : Content() {}

    // require the player to fulfil an expression to take this scene
    // usually only used in Actions
    public record Require(
      Expression Expression)
      : Content() {}


    // something the player can do here
    public record Option(
      Scene Action)
      : Content() {}

    // allow the player to return to another scene without consequence
    public record Return(
      Scene Scene)
      : Content() {}

    // this scene can continue on into another scene
    public record Continue(
      Scene Scene)
      : Content() {}


    // consider a set of settings only if a condition holds
    public record Condition(
      Expression If,
      IImmutableList<Content> Then,
      IImmutableList<Content> Else)
      : Content() {}

    // includes another scene here, completely
    public record Include(
      Scene Scene)
      : Content() {}

  }
}

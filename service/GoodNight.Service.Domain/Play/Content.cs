using System.Collections.Generic;

namespace GoodNight.Service.Domain.Play
{
  public record Content() {}

  // actual text for the player to read. This can be markdown.
  public record Text(
    string Markdown)
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


  // set a quality of the player to a new value
  public record Set(
    string Quality,
    Expression Expression)
    : Content() {}

  // require the player to fulfil an expression to take this scene
  public record Require(
    Expression Expression)
    : Content() {}


  // something the player can do here
  public record Option(
    Action Action)
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
    IEnumerable<Content> Then,
    IEnumerable<Content> Else)
    : Content() {}

  // includes another scene here, completely
  public record Include(
    Scene Scene)
    : Content() {}
}

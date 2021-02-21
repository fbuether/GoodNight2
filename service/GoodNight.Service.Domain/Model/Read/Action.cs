using System.Collections.Immutable;
using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Read
{
  /// <summary>
  /// A Requirement expresses a condition that a Player must pass in order to
  /// be able to choose a specific Option. It is a full Expression that can
  /// evaluate to true or false for the Player, which is given by Passed.
  /// </summary>
  public record Requirement(
    Expression<IStorableReference<Quality, string>> Expression,
    bool Passed)
  {}

  /// <summary>
  /// An Option the player can take as they are at this Action.
  /// It shows a text and optionally an Icon. It may pose a set of Requirements
  /// that a Player state must fulfil in order to be taken, and consequently is
  /// available or not. When taken, it applies a set of effects and will
  /// continue on to a new scene.
  /// </summary>
  public record Option(
    string Name,
    string Text,
    string? Icon,
    bool IsAvailable,
    IImmutableList<Requirement> Requirements,
    IImmutableList<Property> Effects,
    IStorableReference<Scene, string> Scene)
  {}

  /// <summary>
  /// An Action is one scene that a player is currently playing.
  /// It applies the scene to the player state, yielding a finished text, a set
  /// of effects, and a set of options that the player may (or may not) take.
  /// </summary>
  public record Action(
    IStorableReference<Scene, string> Scene,
    string Text,
    IImmutableList<Property> Effects,
    IImmutableList<Option> Options,
    IStorableReference<Scene, string>? Return,
    IStorableReference<Scene, string>? Continue)
    : IStorable<string>
  {
    public string GetKey()
    {
      return Scene.Key;
    }
  }
}

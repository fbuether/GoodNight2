using System.Collections.Immutable;
using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Domain.Model.Write;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Read
{
  /// <summary>
  /// A Requirement expresses a condition that a Player must pass in order to
  /// be able to choose a specific Option. It is a full Expression that can
  /// evaluate to true or false for the Player, which is given by Passed.
  /// </summary>
  public record Requirement(
    Expression Expression,
    bool Passed)
  {}

  /// <summary>
  /// After a player has played a scene, yielding an Action, they have a new set
  /// of possibilities. These may include Options, which continue on to a new
  /// Scene, but have a set of Requirements.
  /// </summary>
  public record Option(
    IStorableReference<Scene, string> Scene,
    bool IsAvailable,
    string Text,
    IImmutableList<Requirement> Requirements)
  {}

  /// <summary>
  /// A Read.Action is one scene as it has been played by a player.
  /// It applies the scene configuration to the player state, yielding a
  /// finished text, a set of effects, and a set of options that the player
  /// may (or may not) take.
  /// </summary>
  public record Action(
    string Name,
    string Text,
    IImmutableList<Property> Effects,
    IImmutableList<Option> Options,
    string? Return,
    string? Continue)
    : IStorable<string>
  {
    public string GetKey()
    {
      return Name;
    }
  }
}

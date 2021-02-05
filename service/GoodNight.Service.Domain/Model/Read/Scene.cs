using System.Collections.Immutable;
using GoodNight.Service.Domain.Model.Expressions;

namespace GoodNight.Service.Domain.Read
{
  public record Requirement(
    Expression Expression,
    bool Passed)
  {}

  public record Option(
    string Scene,
    bool IsAvailable,
    string Text,
    IImmutableList<Requirement> Requirements)
  {}

  /// <summary>
  /// A Read.Scene is one potion of story materialised for a specific player.
  /// It includes all effects this scene had on the player, as well as all
  /// options that this player has from here on.
  /// </summary>
  public record Scene(
    string Urlname,
    string Text,
    IImmutableList<(Quality, Value)> Effects,
    IImmutableList<Option> Options,
    string? Return,
    string? Continue
  ) {}
}

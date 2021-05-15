using System;
using System.Linq;
using System.Collections.Immutable;
using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Domain.Model.Read.Error;

namespace GoodNight.Service.Domain.Model.Read
{
  /// <summary>
  /// A Requirement expresses a condition that a Player must pass in order to
  /// be able to choose a specific Option. It is a full Expression that can
  /// evaluate to true or false for the Player, which is given by Passed.
  /// </summary>
  public record Requirement(
    Expression<IReference<Quality>> Expression,
    bool Passed)
  {
    internal Transfer.Requirement ToTransfer()
    {
      return new Transfer.Requirement(Expression.Map(qualityRef => {
        var quality = qualityRef.Get();
        if (quality is null)
          throw new InvalidQualityException(
            $"Quality \"{qualityRef.Key}\" does not exist.");

        return quality.ToHeader();
      }),
        Passed);
    }
  }

  /// <summary>
  /// An Option the player can take as they are at this Action.
  /// It shows a text and optionally an Icon. It may pose a set of Requirements
  /// that a Player state must fulfil in order to be taken, and consequently is
  /// available or not. When taken, it applies a set of effects and will
  /// continue on to a new scene.
  /// </summary>
  public record Option(
    string Urlname,
    string Text,
    string? Icon,
    bool IsAvailable,
    IImmutableList<Requirement> Requirements,
    IImmutableList<Property> Effects,
    IReference<Scene> Scene)
  {
    internal Transfer.Option ToTransfer() =>
      new Transfer.Option(Urlname, Text, Icon, IsAvailable,
        ImmutableList.CreateRange(Requirements.Select(r => r.ToTransfer())),
        ImmutableList.CreateRange(Effects.Select(e => e.ToTransfer())),
        Scene.Key);
  }

  /// <summary>
  /// An Action is one scene that a player is currently playing.
  /// It applies the scene to the player state, yielding a finished text, a set
  /// of effects, and a set of options that the player may (or may not) take.
  /// </summary>
  public record Action(
    IReference<Scene> Scene,
    string Text,
    IImmutableList<Property> Effects,
    IImmutableList<Option> Options,
    IReference<Scene>? Return,
    IReference<Scene>? Continue)
  {
    public (Log?, Scene?) ContinueWith(string player, uint lastLogNumber,
      string optionname)
    {
      Func<Choice,Log> buildLog = (Choice choice) =>
        new Log(player, lastLogNumber + 1, Scene, Text, Effects, choice);

      if (optionname == "return" && Return != null)
      {
        return (buildLog(new Choice.Return()), Return.Get());
      }
      else if (optionname == "continue" && Continue != null)
      {
        return (buildLog(new Choice.Continue()), Continue.Get());
      }

      var option = Options.FirstOrDefault(o => o.Urlname == optionname);
      if (option != null)
      {
        return (buildLog(new Choice.Action(
              option.Urlname,
              option.Text,
              option.Icon,
              option.Effects)),
          option.Scene.Get());
      }

      return (null, null);
    }

    internal Transfer.Action ToTransfer() =>
      new Transfer.Action(Text,
        ImmutableList.CreateRange(Effects.Select(e => e.ToTransfer())),
        ImmutableList.CreateRange(Options.Select(o => o.ToTransfer())),
        Return?.Key, Continue?.Key);
  }
}

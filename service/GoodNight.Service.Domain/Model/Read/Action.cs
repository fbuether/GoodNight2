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
    Value Value)
  {
    internal Transfer.Requirement ToTransfer()
    {
      var transferExpr = Expression.Map(qualityRef => {
        var quality = qualityRef.Get();
        if (quality is null)
          throw new InvalidQualityException(
            $"Quality \"{qualityRef.Key}\" does not exist.");

        return quality.Name;
      });

      // todo: Grab actual icon of quality.
      return new Transfer.Requirement(transferExpr.Format(q => q), null,
        Value is Value.Bool && ((Value.Bool)Value).Value);
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
    bool IsAvailable,
    IImmutableList<Requirement> Requirements,
    IImmutableList<Property> Effects,
    IReference<Scene> Scene)
  {
    internal Transfer.Option ToTransfer() =>
      new Transfer.Option(Urlname, Text, IsAvailable,
        // ImmutableList.CreateRange<Transfer.Test>(new[] {
        //     new Transfer.Test("foobar1", "pianist", 7),
        //     new Transfer.Test("foobar2", "sundial", 27),
        //     new Transfer.Test("foobar3", "swipe-card", 47),
        //     new Transfer.Test("foobar4", "mining", 67),
        //     new Transfer.Test("toast5", "magic-gate", 83)
        //   }),
        // todo: grab actual list of tests here.
        ImmutableList<Transfer.Test>.Empty,
        ImmutableList.CreateRange(Requirements.Select(r => r.ToTransfer())));

    public override string ToString()
    {
      return $"Option {{Urlname:{Urlname}, Text:{Text}, "
        + $"IsAvailable:{IsAvailable}, Requirements: ["
        + string.Join(", ", Requirements.Select(r => r.ToString()))
        + "], Effects: [" + string.Join(", ", Effects.Select(e => e.ToString()))
        + $"], Scene:{Scene}}}";
    }
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
    /// <summary>
    /// Play an option of this action, which the player currently is at.
    /// </summary>
    /// <param name="optionname">
    /// optionname is the name of the option that the player takes, usually
    /// the name of the next Scene. It may also be the special names
    /// "return" or "continue", when a player choses the respective action of
    /// the Scene.
    /// </param>
    /// <returns>
    /// If a valid option was given, a new log entry as well as the next
    /// Scene the Player is at. Both are null if the option name was invalid,
    /// which should not happen in regular operation.
    /// </returns>
    public (Log?, Scene?) ContinueWith(string user, uint lastLogNumber,
      string optionname)
    {
      Func<Choice,Log> buildLog = (Choice choice) =>
        new Log(user, lastLogNumber + 1, Scene, Text, Effects, choice);

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
              option.Effects)),
          option.Scene.Get());
      }

      return (null, null);
    }

    internal Transfer.Action ToTransfer() =>
      new Transfer.Action(Text,
        ImmutableList.CreateRange(Effects
          .Where(e => !e.IsHidden())
          .Select(e => e.ToTransfer())),
        ImmutableList.CreateRange(Options.Select(o => o.ToTransfer())),
        Return is not null, Continue is not null);

    public override string ToString()
    {
      return $"Action {{Scene:{Scene}, Text:{Text}, Effects: ["
        + string.Join(", ", Effects.Select(e => e.ToString()))
        + "], Options: [" + string.Join(", ", Options.Select(o => o.ToString()))
        + $"], Return:{Return}, Continue:{Continue}}}";
    }
  }
}

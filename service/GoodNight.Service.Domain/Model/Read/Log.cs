using System;
using System.Collections.Immutable;
using System.Linq;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Model.Read
{
  /// <summary>
  /// A Choice is an Option that the player has already taken. It is part of
  /// a history Log entry.
  /// </summary>
  public abstract record Choice
  {
    public record Action(
      string Urlname,
      string Text,
      string? Icon,
      IImmutableList<Property> Effects)
      : Choice
    {
      internal override Transfer.Choice ToTransfer() =>
        new Transfer.Choice("action", Text, Icon,
          ImmutableList.CreateRange(Effects.Select(e => e.ToTransfer())));

      public override string ToString()
      {
        return $"Action {{Urlname:{Urlname}, Text:{Text}, Icon:{Icon}, "
          + "Effects: [" + string.Join(", ", Effects.Select(e => e.ToString()))
          + "]}}";
      }
    }

    public record Return
      : Choice
    {
      internal override Transfer.Choice ToTransfer() =>
        new Transfer.Choice("return", null, null, null);
    }

    public record Continue
      : Choice
    {
      internal override Transfer.Choice ToTransfer() =>
        new Transfer.Choice("continue", null, null, null);
    }

    internal abstract Transfer.Choice ToTransfer();
  }

  /// <summary>
  /// A Log is the protocol of an Action when one of its Options has been taken.
  /// It documents the Action including its text and effects, as well as the
  /// Option that the player chose.
  /// </summary>
  /// <remarks>
  /// Logs are only persisted as part of an Adventure, which is part of a User.
  /// </remarks>
  public record Log(
    string User, // used for the key
    uint Number, // sequential number for all Logs of this player.
    IReference<Scene> Scene,
    string Text,
    IImmutableList<Property> Effects,
    Choice Chosen)
    : IStorable<Log>
  {
    public string Key => NameConverter.Concat(User, Scene.Key,
      Number.ToString());

    internal Transfer.Log ToTransfer() =>
      new Transfer.Log(Number, Text,
        ImmutableList.CreateRange(Effects
          .Where(e => !e.IsHidden())
          .Select(e => e.ToTransfer())),
        Chosen.ToTransfer());

    public override string ToString()
    {
      return $"Log {{User:{User}, Number:{Number}, Scene:{Scene}, Text:{Text}, "
        + "Effects:[" + string.Join(", ", Effects.Select(e => e.ToString()))
        + "], Chosen:{Chosen}}}";
    }
  }
}

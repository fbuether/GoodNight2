using System.Collections.Immutable;
using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Domain.Model.Read.Error;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Model.Read
{
  /// <summary>
  /// A Quality describes any kind of state that a Player may have.
  /// It has a name, icon and type, may be visible to the Player or not, and
  /// optionally has a Scene that it can trigger.
  /// </summary>
  public abstract record Quality(
    string Name,
    string Story, // the urlname of the story.
    string? Icon,
    string Description,
    bool Hidden,
    IReference<Scene>? Scene)
    : IStorable<Quality>
  {
    public record Bool(
      string Name,
      string Story,
      string? Icon,
      string Description,
      bool Hidden,
      IReference<Scene>? Scene)
      : Quality(Name, Story, Icon, Description, Hidden, Scene)
    {
      public override Type Type { get; } = Type.Bool;

      public override Value GetDefault() => new Value.Bool(false);
    }

    /// <summary>
    /// An Integer Quality is a Quality that may have an integer value.
    /// They may optionally have a lowest or highest value that a player may
    /// possess.
    /// The value of 0 is always considered to be not-given.
    /// </summary>
    public record Int(
      string Name,
      string Story,
      string? Icon,
      string Description,
      bool Hidden,
      IReference<Scene>? Scene,

      int? Minimum,
      int? Maximum)
      : Quality(Name, Story, Icon, Description, Hidden, Scene)
    {
      public override Type Type { get; } = Type.Int;

      public override Value GetDefault() => new Value.Int(0);
    }

    /// <summary>
    /// Enum Qualities behave as Int Qualities, as their values are integers.
    /// However, Enums additionally attach a set of names to their values, so
    /// that they appear as a set of textual states that the quality can have.
    /// Enums are implicitly restricted to the values for which a description
    /// exist.
    /// Similar to Integer Qualities, Enums are considered to be absent when
    /// a player has the value 0.
    /// </summary>
    public record Enum(
      string Name,
      string Story,
      string? Icon,
      string Description,
      bool Hidden,
      IReference<Scene>? Scene,

      IImmutableDictionary<int, string> Values)
      : Quality(Name, Story, Icon, Description, Hidden, Scene)
    {
      public override Type Type { get; } = Type.Enum;

      public override Value GetDefault() => new Value.Int(0);
    }


    public virtual Type Type { get; }

    public string Urlname => NameConverter.OfString(Name);

    public string Key => NameConverter.Concat(Story, Urlname);

    public abstract Value GetDefault();


    public Transfer.Quality ToTransfer() => new Transfer.Quality(
      Name, Type, Icon, Description, Hidden);

    public string Render(Value value)
    {
      switch (this, value)
      {
        case (Quality.Bool q, Value.Bool v):
          return v.Value ? "true" : "false";

        case (Quality.Int q, Value.Int v):
          return v.Value.ToString();

        case (Quality.Enum q, Value.Int v):
          return q.Values[v.Value];
      }

      throw new InvalidQualityException(
        $"Quality of type {Type} rendered with value {value}. Quality: {this}");
    }
  }
}

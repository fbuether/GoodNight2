using System.Collections.Immutable;
using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Read
{
  /// <summary>
  /// A Quality describes any kind of state that a Player may have.
  /// It has a name, icon and type, may be visible to the Player or not, and
  /// optionally has a Scene that it can trigger.
  /// </summary>
  public abstract record Quality(
    string Name,
    string? Icon,
    string Description,
    bool IsVisible,
    IStorableReference<Scene, string>? Scene)
    : IStorable<string>
  {
    public record Bool(
      string Name,
      string? Icon,
      string Description,
      bool IsVisible,
      IStorableReference<Scene, string>? Scene)
      : Quality(Name, Icon, Description, IsVisible, Scene)
    {
      public override Type Type { get; } = Type.Bool;
    }

    /// <summary>
    /// An Integer Quality is a Quality that may have an integer value.
    /// They may optionally have a lowest or highest value that a player may
    /// possess.
    /// The value of 0 is always considered to be not-given.
    /// </summary>
    public record Int(
      string Name,
      string? Icon,
      string Description,
      bool IsVisible,
      IStorableReference<Scene, string>? Scene,
      int? Minimum,
      int? Maximum)
      : Quality(Name, Icon, Description, IsVisible, Scene)
    {
      public override Type Type { get; } = Type.Int;
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
      string? Icon,
      string Description,
      bool IsVisible,
      IStorableReference<Scene, string>? Scene,
      IImmutableDictionary<int, string> Values)
      : Quality(Name, Icon, Description, IsVisible, Scene)
    {
      public override Type Type { get; } = Type.Enum;
    }


    public virtual Type Type { get; }

    public string Urlname
    {
      get
      {
        return NameConverter.OfString(Name);
      }
    }

    public string GetKey()
    {
      return Urlname;
    }
  }
}

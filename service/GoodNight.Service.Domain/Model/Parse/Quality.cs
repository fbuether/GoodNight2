using System.Collections.Immutable;

namespace GoodNight.Service.Domain.Model.Parse
{
  public record Quality(
    IImmutableList<Quality.Content> Contents)
  {
    public abstract record Content
    {
      // actual text for the player to read. This can be markdown.
      public record Text(
        string Value)
        : Content;

      // The name of this quality. Must occur on each quality.
      public record Name(
        string Value)
        : Content;

      // The icon for this quality as the filename name of the icon.
      public record Icon(
        string Value)
        : Content;

      // to classify scenes for writing add categories or tags
      // tags are opaque strings
      // several tags may be attached to each scene
      public record Tag(
        string Value)
        : Content;

      // categories behave like a path, similar to a directory
      // each scene may have at most one category
      public record Category(
        IImmutableList<string> Path)
        : Content;

      // the type of this quality.
      public record Type(
        Expressions.Type Value)
        : Content;

      // notes this quality as hidden, so that it is never shown to the player.
      public record Hidden
        : Content;

      // A reference to a scene that can play out this quality.
      public record Scene(
        string Urlname)
        : Content;

      // A name definition of a enum-type quality, adding a descriptive
      // name to the quality for this level.
      public record Level(
        int Number,
        string Description)
        : Content;

      // the minimum value that an int-type quality may have, inclusive.
      public record Minimum(
        int Value)
        : Content;

      // the maximum value that an int-type quality may have, inclusive.
      public record Maximum(
        int Value)
        : Content;
    }
  }
}

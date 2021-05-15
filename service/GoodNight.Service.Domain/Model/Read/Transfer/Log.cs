using System.Collections.Immutable;

namespace GoodNight.Service.Domain.Model.Read.Transfer
{
  public abstract record Choice(string Kind)
  {
    public record Action(
      string Text,
      string? Icon,
      IImmutableList<Property> Effects)
      : Choice("action");

    public record Return()
      : Choice("return");

    public record Continue()
      : Choice("continue");
  }

  public record Log(
    uint Number,
    string Text,
    IImmutableList<Property> Effects,
    Choice Chosen);
}

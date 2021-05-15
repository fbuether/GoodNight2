using System.Collections.Immutable;

namespace GoodNight.Service.Domain.Model.Read.Transfer
{
  public abstract record Choice
  {
    public record Action(
      string Text,
      string? Icon,
      IImmutableList<Property> Effects)
      : Choice;

    public record Return
      : Choice;

    public record Continue
      : Choice;
  }

  public record Log(
    uint Number,
    string Text,
    IImmutableList<Property> Effects,
    Choice Chosen);
}

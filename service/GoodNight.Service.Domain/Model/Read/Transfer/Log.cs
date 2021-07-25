using System.Collections.Immutable;

namespace GoodNight.Service.Domain.Model.Read.Transfer
{
  public record Choice(
    string Kind,
    string? Text,
    string? Icon,
    IImmutableList<Property>? Effects);

  public record Log(
    uint Number,
    string Text,
    IImmutableList<Property> Effects,
    Choice Chosen);
}

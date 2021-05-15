using System.Collections.Immutable;

namespace GoodNight.Service.Domain.Model.Read.Transfer
{
  public record Player(
    string Name,
    IImmutableList<Property> State);
}

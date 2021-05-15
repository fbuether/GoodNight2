using System.Collections.Immutable;

namespace GoodNight.Service.Domain.Model.Read.Transfer
{
  public record Adventure(
    Player Player,
    IImmutableList<Log> History,
    Action Current);
}

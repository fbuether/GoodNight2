using System.Collections.Immutable;

namespace GoodNight.Service.Domain.Read
{
  public record Adventure(
    Player Player,
    Story Story,
    IImmutableList<Action> History,
    Scene Current)
  {}
}

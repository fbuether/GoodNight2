using GoodNight.Service.Domain.Model.Expressions;

namespace GoodNight.Service.Domain.Read
{
  /// <summary>
  /// A Poperty associates a Quality with one of its possible Values. It is used
  /// to construct a Player's state, or to describe effects of a Choice.
  /// </summary>
  public record Property(
    Quality Quality,
    Value Value)
  {}
}

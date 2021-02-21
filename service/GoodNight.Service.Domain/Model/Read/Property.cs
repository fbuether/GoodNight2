using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Model.Read
{
  /// <summary>
  /// A Property associates a Quality with one of its possible Values. It is
  /// used to construct a Player's state, or to describe effects of a Choice.
  /// </summary>
  /// <remarks>
  /// Properties are only persisted as parts of their parent elements.
  /// </remarks>
  public record Property(
    IStorableReference<Quality, string> Quality,
    Value Value)
  {}
}

using System;
using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Domain.Model.Read.Error;
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
    IReference<Quality> Quality,
    Value Value)
  {
    internal Transfer.Property ToTransfer()
    {
      var quality = Quality.Get();
      if (quality is null)
        throw new InvalidQualityException(
          $"Quality \"{Quality.Key}\" does not exist.");

      return new Transfer.Property(quality.ToTransfer(), quality.Render(Value));
    }
  }
}

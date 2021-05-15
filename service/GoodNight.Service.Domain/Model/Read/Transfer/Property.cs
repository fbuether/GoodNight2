using GoodNight.Service.Domain.Model.Expressions;

namespace GoodNight.Service.Domain.Model.Read.Transfer
{
  public record Property(
    QualityHeader Quality,
    Value Value);
}

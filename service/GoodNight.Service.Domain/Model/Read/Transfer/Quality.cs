using GoodNight.Service.Domain.Model.Expressions;

namespace GoodNight.Service.Domain.Model.Read.Transfer
{
  /// <summary>
  /// A QualityHeader is a short summary of a quality. It provides the
  /// most important information in a format suitable for serialisation.
  /// </summary>
  public record Quality(
    string Name,
    Type Type,
    string? Icon,
    string Description,
    bool Hidden);
}

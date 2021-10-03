using System.Collections.Generic;

namespace GoodNight.Service.Domain.Model.Write.Transfer
{
  public record Scene(
    string Name,
    string Story,

    string Raw,

    IEnumerable<Reference> OutLinks,
    IEnumerable<Reference> InLinks,
    IEnumerable<Reference> Qualities);
}

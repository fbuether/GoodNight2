using System.Collections.Generic;

namespace GoodNight.Service.Domain.Play
{
  public record Action(
    IEnumerable<Content> Content)
  {}
}

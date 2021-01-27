using System.Collections.Generic;

namespace GoodNight.Service.Domain.Story
{
  public record Action(
    IEnumerable<Content> Content)
  {}
}

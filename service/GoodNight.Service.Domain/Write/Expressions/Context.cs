using System.Collections.Immutable;

namespace GoodNight.Service.Domain.Write.Expressions
{
  public interface IContext : IImmutableDictionary<string, Value>
  {
  }
}

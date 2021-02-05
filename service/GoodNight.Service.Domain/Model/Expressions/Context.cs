using System.Collections.Immutable;

namespace GoodNight.Service.Domain.Model.Expressions
{
  public interface IContext : IImmutableDictionary<string, Value>
  {
  }
}

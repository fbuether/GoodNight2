using System.Collections.Immutable;

namespace GoodNight.Service.Domain.Play.Expressions
{
  public interface IContext : IImmutableDictionary<string, Value>
  {
  }
}

using System.Collections.Immutable;
using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Domain.Util;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Model.Write
{
  public record Quality(
    string Name,
    string Story, // urlname of the story, for the key

    string? Icon,
    string Raw,
    IImmutableList<string> Tags,
    IImmutableList<string> Category)
    : IStorable<Quality>
  {
    public string Urlname => NameConverter.OfString(Name);

    public string Key => NameConverter.Concat(Story, Urlname);
  }
}

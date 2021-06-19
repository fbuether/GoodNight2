using System.Collections.Immutable;
using System.Linq;
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

    public override string ToString()
    {
      return $"Quality {{Name:{Name}, Story:{Story}, Icon:{Icon}, "
        + $"Raw:\"{Raw}\", Tags: [" + string.Join(", ", Tags)
        + "], Category: " + string.Join("/", Category) + "}";
    }
  }
}


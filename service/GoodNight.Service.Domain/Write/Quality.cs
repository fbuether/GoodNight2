using GoodNight.Service.Domain.Write.Expressions;

namespace GoodNight.Service.Domain.Write
{
  public record Quality(
    string Name,
    string Raw,
    Type Type
    // string Image
  )
  {
    public string Urlname
    {
      get
      {
        return NameConverter.OfString(Name);
      }
    }


    public record Bool(
      string Name,
      string Raw)
      : Quality(Name, Raw, Type.Bool) {}

    public record Int(
      string Name,
      string Raw)
      : Quality(Name, Raw, Type.Int) {}
  }
}

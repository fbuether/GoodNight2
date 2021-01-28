
namespace GoodNight.Service.Domain.Write
{
  public record Quality(
    string Name)
  {
    public string Urlname
    {
      get
      {
        return NameConverter.OfString(Name);
      }
    }
  }
}

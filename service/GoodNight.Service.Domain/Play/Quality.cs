
namespace GoodNight.Service.Domain.Play
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

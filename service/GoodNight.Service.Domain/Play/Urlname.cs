using System.Text.RegularExpressions;

namespace GoodNight.Service.Domain.Play
{
  public static class NameConverter
  {
    public static string OfString(string Name)
    {
      return Regex.Replace(Name, "[^a-zA-Z0-9]", "-").Trim().ToLower();
    }
  }
}

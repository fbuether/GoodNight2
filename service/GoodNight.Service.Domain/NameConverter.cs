using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace GoodNight.Service.Domain
{
  public static class NameConverter
  {
    public static string OfString(string Name)
    {
      return Regex.Replace(Name, "[^a-zA-Z0-9]", "-").Trim().ToLower();
    }

    public static string Concat(params string[] parts)
    {
      return string.Join("/", parts.Select(OfString));
    }
  }
}

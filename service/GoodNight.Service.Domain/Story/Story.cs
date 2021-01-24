using System;
using System.Text.RegularExpressions;

namespace GoodNight.Service.Domain.Story
{
  public record Story (
    string Name)
  {
    public string Urlname {
      get {
        return Regex.Replace(Name, "[^a-zA-Z0-9]", "-").Trim().ToLower();
      }
    }
  }
}

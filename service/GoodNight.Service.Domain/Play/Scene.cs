using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GoodNight.Service.Domain.Play
{
  public record Scene (
    string Name,
    IEnumerable<Content> Content)
  {
    public string Urlname {
      get {
        return Regex.Replace(Name, "[^a-zA-Z0-9]", "-").Trim().ToLower();
      }
    }
  }
}

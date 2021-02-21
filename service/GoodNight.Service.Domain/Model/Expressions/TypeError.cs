using System;

namespace GoodNight.Service.Domain.Model.Expressions
{
  public class TypeError : Exception
  {
    public TypeError(string description)
      : base(description)
    {}
  }
}

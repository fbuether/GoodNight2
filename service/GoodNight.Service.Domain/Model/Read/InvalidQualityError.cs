using System;

namespace GoodNight.Service.Domain.Model.Read
{
  public class InvalidQualityError : Exception
  {
    public InvalidQualityError(string description)
      : base(description)
    {}
  }
}

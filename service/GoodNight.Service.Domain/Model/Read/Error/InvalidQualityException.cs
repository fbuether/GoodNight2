using System;

namespace GoodNight.Service.Domain.Model.Read.Error
{
  public class InvalidQualityException : Exception
  {
    public InvalidQualityException(string description)
      : base(description)
    {}
  }
}

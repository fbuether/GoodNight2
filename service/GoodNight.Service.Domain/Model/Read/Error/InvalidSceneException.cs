using System;

namespace GoodNight.Service.Domain.Model.Read.Error
{
  public class InvalidSceneException : Exception
  {
    public InvalidSceneException(string description)
      : base(description)
    {}
  }
}

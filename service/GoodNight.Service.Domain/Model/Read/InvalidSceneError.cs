using System;

namespace GoodNight.Service.Domain.Model.Read
{
  public class InvalidSceneError : Exception
  {
    public InvalidSceneError(string description)
      : base(description)
    {}
  }
}

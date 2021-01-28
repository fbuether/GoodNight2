
namespace GoodNight.Service.Domain.Play.Expressions
{
  public record Value()
  {
    public record Bool(
      bool Value)
      : Value() {}

    public record Number(
      int Value) 
      : Value() {}
  }
}

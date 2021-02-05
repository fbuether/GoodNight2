
namespace GoodNight.Service.Domain.Model.Expressions
{
  public record Value()
  {
    public record Bool(
      bool Value)
      : Value() {}

    public record Number(
      int Value) 
      : Value() {}

    public record Enum(
      int Value)
      : Number(Value) {}
  }
}

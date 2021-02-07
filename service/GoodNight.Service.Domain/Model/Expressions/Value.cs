
namespace GoodNight.Service.Domain.Model.Expressions
{
  public record Value
  {
    public record Bool(
      bool Value)
      : Value() {}

    public record Int(
      int Value) 
      : Value() {}

    public record Enum(
      int Value)
      : Int(Value) {}
  }
}

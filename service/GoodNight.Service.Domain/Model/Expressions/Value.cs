
namespace GoodNight.Service.Domain.Model.Expressions
{
  public interface Value
  {
    public record Bool(
      bool Value)
      : Value;

    public record Int(
      int Value)
      : Value;
  }
}

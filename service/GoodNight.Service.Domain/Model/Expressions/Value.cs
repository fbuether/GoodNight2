
namespace GoodNight.Service.Domain.Model.Expressions
{
  public interface Value
  {
    public bool isEmpty();

    public record Bool(
      bool Value)
      : Value
    {
      public bool isEmpty() => !Value;
    }

    public record Int(
      int Value)
      : Value
    {
      public bool isEmpty() => Value == 0;
    }
  }
}

using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Domain.Model.Write;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Read
{
  /// <summary>
  /// A Quality describes any kind of state that a Player may have.
  /// It has a name and type, as well as optionally a Scene that it can trigger.
  /// </summary>
  public abstract record Quality(
    string Name,
    Type Type,
    string? Icon,
    string Description,
    IStorableReference<Scene, string>? Scene)
    : IStorable<string>
  {
    public record Bool(
      string Name,
      string? Icon,
      string Description,
      IStorableReference<Scene, string>? Scene)
      : Quality(Name, Type.Bool, Icon, Description, Scene) {}

    public record Int(
      string Name,
      string? Icon,
      string Description,
      IStorableReference<Scene, string>? Scene)
      : Quality(Name, Type.Int, Icon, Description, Scene) {}

    public record Enum(
      string Name,
      string? Icon,
      string Description,
      IStorableReference<Scene, string>? Scene)
      : Quality(Name, Type.Enum, Icon, Description, Scene) {}


    public string Urlname
    {
      get
      {
        return NameConverter.OfString(Name);
      }
    }

    public string GetKey()
    {
      return Urlname;
    }
  }
}

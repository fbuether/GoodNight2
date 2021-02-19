using System.Runtime.Serialization;

namespace GoodNight.Service.Storage.Interface
{
  /// <summary>
  /// An object implementing IStorable can be persisted in a Storage.IStore.
  /// This is an aggregate root in DDD nomenclature.
  /// </summary>
  public interface IStorable : ISerializable
  {

  }
}


namespace GoodNight.Service.Storage.Interface
{
  /// <summary>
  /// An object implementing IStorable can be persisted in a Storage.IStore.
  /// This is an aggregate root in DDD nomenclature.
  /// </summary>
  public interface IStorable
  {
    /// <summary>
    /// Returns the key of this object as a string.
    /// </summary>
    /// <returns>
    /// The key of this object.
    /// </returns>
    string GetKey();
  }
}

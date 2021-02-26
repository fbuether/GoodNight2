
namespace GoodNight.Service.Storage.Interface
{
  /// <summary>
  /// An object implementing IStorable can be persisted in a Storage.IStore.
  /// This is an aggregate root in DDD nomenclature.
  /// </summary>
  /// <typeparam name="K">
  /// The type of the keys of this type.
  /// </typeparam>
  public interface IStorable<K>
    where K : notnull
  {
    /// <summary>
    /// Fetches the key of this object.
    /// </summary>
    /// <returns>
    /// The key of this object.
    /// </returns>
    K GetKey();
  }
}

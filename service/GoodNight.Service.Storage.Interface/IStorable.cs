
namespace GoodNight.Service.Storage.Interface
{
  /// <summary>
  /// An object implementing IStorable can be persisted in a Storage.IStore.
  /// This is an aggregate root in DDD nomenclature.
  /// </summary>
  /// <remarks>
  /// Storables automatically are references to themselves. They provide their
  /// Key, and can return themselves as the full objects if requested.
  ///
  /// Objects may be used as references when they are added to the storable
  /// object tree at a place where a reference is expected. They will be added
  /// to the appropriate repository as soon as they are persisted, i.e. with the
  /// next call to `Add`, `Update` or `Save` of an object tree parent.
  /// </remarks>
  public interface IStorable<T> : IReference<T>
    where T : class, IStorable<T>
  {
    /// <summary>
    /// Return the value that this IReference points to.
    /// </summary>
    /// <returns>
    /// </returns>
    T? IReference<T>.Get() => this as T;
  }
}

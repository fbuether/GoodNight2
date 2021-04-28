
namespace GoodNight.Service.Storage.Interface
{
  /// <summary>
  /// IStorableReferences describe a link to another IStorable object group
  /// root.
  /// Usually, the storage holds each object with all its subordinates, that is,
  /// it stores the full object graph. The object may contain references to
  /// other root objects, which storage does not detect on its own.
  /// To mark references to other root objects, the value of the root object
  /// must be an IStorableReference.
  /// </summary>
  /// <typeparam name="T">
  /// The type of the element to store.
  /// </typeparam>
  /// <remarks>
  /// This is a rather thin object, and contains only its key,
  /// the backing repository, and possibly the object it points to.
  /// </remarks>
  public interface IReference<T>
    where T : class, IStorable<T>
  {
    /// <summary>
    /// Returns the key of this reference.
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Resolves this reference in the store it was created in.
    /// </summary>
    /// <returns>
    /// The element associated with the saved key in this store, or null if this
    /// reference is invalid, that is, the key is not assigned in the store.
    /// </returns>
    T? Get();
  }
}


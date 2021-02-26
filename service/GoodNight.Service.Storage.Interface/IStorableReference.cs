
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
  /// This is a rather thin object, and contains only pointers to its key,
  /// the backing repository, and possibly the object it points to.
  /// </remarks>
  public interface IStorableReference<T, K>
    where T : IStorable<K>
    where K : notnull
  {
    /// <summary>
    /// Returns the key of this reference.
    /// </summary>
    K Key { get; }

    // /// <summary>
    // /// Sets this element to reference a specific element in the store.
    // /// </summary>
    // public void Set(K key);

    /// <summary>
    /// Resolves this reference in the store it was created in.
    /// </summary>
    /// <returns>
    /// The element associated with the saved key in this store, or null if the
    /// key does not point to an element.
    /// </returns>
    T? Get();
  }
}


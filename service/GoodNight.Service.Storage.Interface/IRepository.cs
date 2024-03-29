using System;
using System.Collections.Generic;

namespace GoodNight.Service.Storage.Interface
{
  /// <summary>
  /// A IRepository persistently stores elements of a specific type.
  ///
  /// Each element is stored at a unique string key, to allow fast
  /// lookup. This key behaves similar to a primary key in a
  /// relational database. Changes to the store persist in a backing
  /// store, e.g. a file. It is associated when the IRepository gets
  /// created by `IStore:Create`.
  /// </summary>
  /// <typeparam name="T">
  /// The type of the elements to store.
  /// </typeparam>
  public interface IRepository<T> : IReadOnlyCollection<T>
    where T : class, IStorable<T>
  {
    /// <summary>
    /// Add a new element to the store.
    /// Requires a key by which the element may be looked up, and the element
    /// itself.
    /// If the key of element already exists in the store, it is replaced with
    /// this element, and all references to it are updated to this.
    /// </summary>
    /// <param name="key">
    /// A unique key to store the element to be added at.
    /// </param>
    /// <param name="element">
    /// The element to be stored.
    /// </param>
    /// <returns>
    /// A reference to the new element, or null if the element already exists.
    /// </returns>
    /// <remarks>
    /// The item is immediately present in this store; however, it may take some
    /// time for it to be safely persisted to disk. Use `IStore:Sync()` to be
    /// sure that it is written.
    /// </remarks>
    IReference<T>? Add(T element);

    /// <summary>
    /// Load an element from the store.
    /// </summary>
    /// <param name="key">
    /// The unique key of the element to get.
    /// </param>
    /// <returns>
    /// The element associated with the key, or null if no element with the key
    /// is stored here.
    /// </returns>
    T? Get(string key);

    /// <summary>
    /// Creates a reference to a specified key, even if the element does not
    /// exist in the repository.
    /// </summary>
    /// <param name="key">
    /// The unique key of the element to create the reference for.
    /// </param>
    /// <returns>
    /// A reference to the element with the given key.
    /// </returns>
    IReference<T> GetReference(string key);


    /// <summary>
    /// Adds or replaces this element. If its key does not yet exist, it is
    /// added, otherwise the existing element is replaced with this.
    /// </summary>
    /// <param name="element">
    /// The element to be stored.
    /// </param>
    /// <returns>
    /// The reference to the saved element.
    /// </returns>
    IReference<T> Save(T element);


    /// <summary>
    /// Replaces the element with key K, if it exists in the store, ignoring the
    /// existing element.
    /// This is guaranteed to always replace the current instance of element,
    /// and will be handled like an atomic operation. Consequently, `update` may
    /// not be async.
    /// </summary>
    /// <param name="element">
    /// The element that should replace the existing element with the same key.
    /// </param>
    /// <returns>
    /// A reference to the replaced element, or null if no element with the same
    /// key did exist.
    /// </returns>
    IReference<T>? Update(T element);

    /// <summary>
    /// Mutates the element with key K, if it exists in the store, and replaces
    /// the stored version with the new version.
    /// This is guaranteed to always update the current instance of element, and
    /// will be handled like an atomic operation. Consequently, `update` may not
    /// be async.
    /// </summary>
    /// <param name="key">
    /// The unique key of the element to be updated.
    /// </param>
    /// <param name="update">
    /// A function to update the element with, if it exists. If the function
    /// returns null, the update will be canceled, and the old value remains
    /// in storage.
    /// </param>
    /// <returns>
    /// A reference to the update element, or null if `key` did not exist or
    /// `update` returned null.
    /// </returns>
    IReference<T>? Update(string key, Func<T,T?> update);

    /// <summary>
    /// Mutates the element with key K, if it exists in the store, and replaces
    /// the stored version with the new version.
    /// This is guaranteed to always update the current instance of element, and
    /// will be handled like an atomic operation. Consequently, `update` may not
    /// be async.
    /// </summary>
    /// <param name="key">
    /// The unique key of the element to be updated.
    /// </param>
    /// <param name="update">
    /// A function to update the element with, if it exists. The function must
    /// return the new value along with an arbitrary result.
    /// </param>
    /// <returns>
    /// The result of `update`, or null if `key` did not exist or `update`
    /// returned null.
    /// </returns>
    U? Update<U>(string key, Func<T, (T, U)?> update)
      where U : class;


    /// <summary>
    /// Removes the element associated with the given key.
    /// </summary>
    /// <param name="key">
    /// The unique key of the element to be removed.
    /// </param>
    /// <returns>
    /// true if the element existed, and false otherwise.
    /// </returns>
    bool Remove(string key);
  }
}

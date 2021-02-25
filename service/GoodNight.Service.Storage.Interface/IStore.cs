using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoodNight.Service.Storage.Interface
{
  /// <summary>
  /// A store persists several sets of object groups.
  /// </summary>
  public interface IStore
  {
    /// <summary>
    /// Add a new element to the store.
    /// Requires a key by which the element may be looked up, and the element
    /// itself.
    /// If the key of element already exists in the store, it is replaced with
    /// this element, and all references to it are updated to this.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the element to store.
    /// </typeparam>
    /// <typeparam name="K">
    /// The type of the key to associate the element with. This type must always
    /// be the same for all elements of the same type <typeparamref name="T" />.
    /// </typeparam>
    /// <param name="key">
    /// A unique key to store the element to be added at.
    /// </param>
    /// <param name="element">
    /// The element to be stored.
    /// </param>
    public Task Add<T, K>(T element)
      where T : class, IStorable<K>
      where K : notnull;

    /// <summary>
    /// Load an element from the store.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the element to get.
    /// </typeparam>
    /// <typeparam name="K">
    /// The type of the key to find the element by.
    /// </typeparam>
    /// <param name="key">
    /// The key of the element to get.
    /// </param>
    /// <returns>
    /// The element associated with the key, or null if the key could
    /// not be found.
    /// </returns>
    public T? Get<T, K>(K key)
      where T : class, IStorable<K>
      where K : notnull;

    /// <summary>
    /// Mutates the element with key K, if it exists.
    /// This is guaranteed to always update the current instance of element, and
    /// will be handled like an atomic operation. Consequently, `update` may not
    /// be async.
    /// </summary>
    /// <returns>
    /// The new element, or null if `key` did not exist.
    /// </returns>
    public T? Update<T, K>(K key, Func<T, T> update)
      where T : class, IStorable<K>
      where K : notnull;

    /// <summary>
    /// Mutates the element with key K, if it exists.
    /// This is guaranteed to always update the current instance of element, and
    /// will be handled like an atomic operation. Consequently, `update` may not
    /// be async.
    /// If update returns null, the element will not be changed.
    /// </summary>
    /// <returns>
    /// The new element, or null if `key` did not exist.
    /// </returns>
    public U? WithUpdate<T, K, U>(K key, Func<T, (T, U)?> update)
      where T : class, IStorable<K>
      where K : notnull
      where U : class;

    /// <summary>
    /// Fetches all objects of a specific type from the store.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the elements to get.
    /// </typeparam>
    /// <returns>
    /// All elements of the specific type. If no elements exist, this returns
    // an empty enumeration.
    /// </returns>
    public IEnumerable<T> GetAll<T, K>()
      where T : class, IStorable<K>
      where K : notnull;
  }
}

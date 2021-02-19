using System.Collections.Generic;

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
    /// <returns>
    /// A new store containing everything of the original store as well as the
    /// new value.
    /// </returns>
    public IStore Add<T,K>(K key, T element)
      where T : struct, IStorable
      where K : struct, IStorableKey;

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
    public T? Get<T,K>(K key)
      where T : struct, IStorable
      where K : struct, IStorableKey;


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
    public IEnumerable<T> GetAll<T>()
      where T : IStorable;
  }
}

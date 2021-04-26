using System;

namespace GoodNight.Service.Storage.Interface
{
  /// <summary>
  /// A store persists several sets of object groups.
  /// </summary>
  /// <remarks>
  /// When disposing this store, it also disposes all IRepositories it has
  /// created.
  /// </remarks>
  public interface IStore : IDisposable
  {
    /// <summary>
    /// Creates a new store for a specific type.
    ///
    /// It uses the given backing stream as persistence for its data. If the
    /// given stream already contains data, this will read the data from the
    /// stream, and append new operations to the end of it.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the elements to store.
    /// </typeparam>
    /// <typeparam name="K">
    /// The type of the keys to store elements at. This may not be nullable,
    /// and ideally should have a good hash function.
    /// </typeparam>
    /// <param name="uniqueName">
    /// Each repository requires a unique name that identifies the objects of
    /// itself. The store uses this name to discern entries in the journal, esp.
    /// when recovering an older journal.
    /// </param>
    /// <returns>
    /// A new store for elements of the given type using the given stream as
    /// backing store and containing data as present on the stream.
    /// </returns>
    IRepository<T> Create<T>()
      where T : class, IStorable;
  }
}

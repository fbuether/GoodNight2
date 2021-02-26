using System;
using System.IO;
using System.Threading.Tasks;

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
    /// <param name="backingStore">
    /// A stream onto the backing store. The stream must be able to seek to
    /// start and end. If this stream already contains data, the data will
    /// be used to initialise this store. The store assumes exclusive access
    /// to the stream as well as the underlying data. It will dispose the stream
    /// when either this store or the repository is disposed.
    /// </param>
    /// <returns>
    /// A new store for elements of the given type using the given stream as
    /// backing store and containing data as present on the stream.
    /// </returns>
    IRepository<T,K> Create<T,K>(Stream backingStore)
      where T : class, IStorable<K>
      where K : notnull;

    /// <summary>
    /// Creates a new store for a specific type.
    ///
    /// This calls `Create(Stream)` with a file for backing storage, where
    /// the filename is given as `store-{uniqueName}.json`.
    /// </summary>
    IRepository<T,K> Create<T,K>(string uniqueName)
      where T : class, IStorable<K>
      where K : notnull;


    /// <summary>
    /// Returns a Task that waits for all currently outstanding I/O to finish.
    /// Await this right after store operations to ensure that they are safely
    /// persisted.
    /// </summary>
    Task Sync();
  }
}

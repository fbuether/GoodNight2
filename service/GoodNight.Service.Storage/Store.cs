using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Storage.Journal;
using GoodNight.Service.Storage.Serialisation;

namespace GoodNight.Service.Storage
{
  public class Store : IStore, IDisposable
  {
    private static readonly string storeFile = "storage/store.json";

    private Stream backingStore;

    private bool ownsBackingStore = false;

    private List<BaseRepository> repositories;

    internal JsonSerializerOptions JsonSerializerOptions { get; } =
      new JsonSerializerOptions();

    private JournalWriter writer;

    /// <param name="backingStore">
    /// A stream onto the backing store. The stream must be able to seek to
    /// start and end. If this stream already contains data, the data will
    /// be used to initialise this store. The store assumes exclusive access
    /// to the stream as well as the underlying data. It will dispose the stream
    /// when either this store or the repository is disposed.
    /// </param>
    public Store(IEnumerable<JsonConverter> converters,
      Stream? backingStore = null)
    {
      ownsBackingStore = backingStore is null;
      if (backingStore is null)
      {
        var cwd = Directory.GetCurrentDirectory();
        Console.WriteLine($"Store: Using {cwd}/{storeFile}");
        this.backingStore = File.Open(storeFile, FileMode.OpenOrCreate);
      }
      else {
        this.backingStore = backingStore;
      }

      repositories = new List<BaseRepository>();

      // do not write null values, to save space.
      JsonSerializerOptions.DefaultIgnoreCondition =
        JsonIgnoreCondition.WhenWritingNull;

      JsonSerializerOptions.Converters.Add(new ReferenceConverterFactory(this));
      foreach (var converter in converters)
      {
        JsonSerializerOptions.Converters.Add(converter);
      }

      writer = new JournalWriter(JsonSerializerOptions, this.backingStore);
    }

    public void Dispose()
    {
      ((IDisposable)writer).Dispose();

      backingStore.Flush();

      if (ownsBackingStore)
      {
        ((IDisposable)backingStore).Dispose();
      }
    }


    IImmutableDictionary<string, string> IStore.GetStatus()
    {
      var map = ImmutableDictionary.Create<string,string>();
      map = map.Add("Number of Repositories", repositories.Count.ToString());
      foreach (var repos in repositories)
      {
        map = map.Add($"Repository \"{repos.ValueType.Name}\" count",
          (repos as IEnumerable)?.Cast<object>().Count().ToString() ?? "-");
      }

      var storeSize = new System.IO.FileInfo(storeFile).Length;
      var sizeUnit = "B";
      if (storeSize > 10240)
      {
        storeSize = storeSize / 1024;
        sizeUnit = "KiB";
      }
      if (storeSize > 10240)
      {
        storeSize = storeSize / 1024;
        sizeUnit = "MiB";
      }
      if (storeSize > 10240)
      {
        storeSize = storeSize / 1024;
        sizeUnit = "GiB";
      }

      map = map.Add("Store file size", storeSize.ToString() + sizeUnit);

      map = map.Add("Last write time",
        writer.GetLastWrite().ToString("u"));
      map = map.Add("Writer exceptions",
        writer.GetExceptions().Count().ToString());
      foreach (var msg in writer.GetExceptions())
      {
        map = map.Add($"Exception", msg);
      }

      return map;
    }


    /// <remarks>
    /// This method is not async, as it should be executed prior to web service
    /// launch, and should be executed synchronously.
    /// </remarks>
    public void StartJournal()
    {
      JournalReader.ReadAll(backingStore, this, JsonSerializerOptions);
      writer.StartWriting(backingStore);
    }

    private Repository<T> CreateRepository<T>(string typeName)
      where T : class, IStorable<T>
    {
      var existing = repositories.Find(repos => repos.TypeName == typeName);
      if (existing is Repository<T> existingRepos)
        return existingRepos;

      var repos = new Repository<T>(this, writer, typeName);
      repositories.Add(repos);
      return repos;
    }

    // used by journal reader to find the appropriate repository for an entry.
    internal BaseRepository GetRepository(string typeName)
    {
      var elementType = Type.GetType(typeName);
      if (elementType is null)
        throw new Exception(
          $"Cannot create repository for invalid type: {typeName}");

      // call via reflection.
      var method = typeof(Store).GetMethod(nameof(CreateRepository),
        BindingFlags.NonPublic | BindingFlags.Instance);
      if (method is null)
        throw new Exception("Store does not have CreateRepository method.");

      var callable = method.MakeGenericMethod(elementType);
      var repos = callable.Invoke(this, new[] { typeName }) as BaseRepository;
      if (repos is null)
        throw new Exception("CreateRepository did not return a repository.");

      return repos;
    }

    internal Repository<T> GetRepository<T>(string typeName)
      where T : class, IStorable<T>
    {
      var repos = GetRepository(typeName) as Repository<T>;
      if (repos is null)
        throw new Exception($"Could not get repository for {typeName}.");

      return repos;
    }

    IRepository<T> IStore.Create<T>()
    {
      var typeName = typeof(T).AssemblyQualifiedName;
      if (typeName is null)
        throw new Exception(
          "Cannot store types without AssemblyQualifiedName.");

      return CreateRepository<T>(typeName);
    }
  }
}

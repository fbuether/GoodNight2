using System;

namespace GoodNight.Service.Storage
{
  /// <summary>
  /// A descriptor for a stored object. This makes the object discoverable in a
  /// generic way.
  /// </summary>
  internal class StoredObject
  {
    internal object Data { get; set; }

    internal string Type { get; set; }

    internal StoredObject(object data)
    {
      Data = data;

      var typename = data.GetType().FullName;
      if (typename == null)
      {
        throw new InvalidOperationException(
          "Created StoredObject with data of invalid type.");
      }

      Type = typename;
    }
  }
}

using System;
using GoodNight.Service.Storage.Journal;

namespace GoodNight.Service.Storage
{
  internal abstract class BaseRepository
  {
    internal string UniqueName { init; get; }

    public BaseRepository(string uniqueName)
    {
      UniqueName = uniqueName;
    }

    internal abstract Type KeyType { get; }

    internal abstract Type ValueType { get; }

    internal abstract bool Replay(Entry entry);
  }
}

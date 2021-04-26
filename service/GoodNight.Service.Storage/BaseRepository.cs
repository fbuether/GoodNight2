using System;
using GoodNight.Service.Storage.Journal;

namespace GoodNight.Service.Storage
{
  internal abstract class BaseRepository
  {
    internal string TypeName { init; get; }

    public BaseRepository(string typeName)
    {
      TypeName = typeName;
    }

    internal abstract Type ValueType { get; }

    internal abstract bool Replay(Entry entry);
  }
}

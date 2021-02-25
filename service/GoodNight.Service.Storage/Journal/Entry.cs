
using System;

namespace GoodNight.Service.Storage.Journal
{
  internal record Entry(Entry.EntryType Type,
    Type KeyType,
    Type ValueType)
  {
    internal enum EntryType
    {
      Add = 1,
      Update = 2,
      Delete = 3
    }

    internal record Add(
      Type KeyType,
      Type ValueType,
      object Key,
      object Value)
      : Entry(EntryType.Add, KeyType, ValueType)
    {}

    internal record Update(
      Type KeyType,
      Type ValueType,
      object Key,
      object Value)
      : Entry(EntryType.Add, KeyType, ValueType)
    {}

    internal record Delete(
      Type KeyType,
      Type ValueType,
      object Key)
      : Entry(EntryType.Add, KeyType, ValueType)
    {}
  }
}

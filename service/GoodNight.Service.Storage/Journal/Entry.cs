
using System;

namespace GoodNight.Service.Storage.Journal
{
  internal record Entry<T,K>(Entry<T,K>.EntryType Type)
  {
    internal enum EntryType
    {
      Add = 1,
      Update = 2,
      Delete = 3
    }

    internal record Add(
      K Key,
      T Value)
      : Entry<T,K>(EntryType.Add)
    {}

    internal record Update(
      K Key,
      T Value)
      : Entry<T,K>(EntryType.Update)
    {}

    internal record Delete(
      K Key)
      : Entry<T,K>(EntryType.Delete)
    {}
  }
}

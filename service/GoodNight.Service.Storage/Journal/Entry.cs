
using System;

namespace GoodNight.Service.Storage.Journal
{
  internal record Entry(string Repos, Entry.EntryType Type)
  {
    internal enum EntryType
    {
      Add = 1,
      Update = 2,
      Delete = 3
    }

    internal record Add(
      string Repos,
      object Value)
      : Entry(Repos, EntryType.Add)
    {}

    internal record Update(
      string Repos,
      string Key,
      object Value)
      : Entry(Repos, EntryType.Update)
    {}

    internal record Delete(
      string Repos,
      string Key)
      : Entry(Repos, EntryType.Delete)
    {}
  }
}

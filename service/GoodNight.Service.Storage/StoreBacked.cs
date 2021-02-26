
using System;
using System.IO;
using System.Threading.Tasks;

namespace GoodNight.Service.Storage
{
  internal abstract class StoreBacked : IDisposable
  {
    protected Stream backingStore { init; get; }

    internal StoreBacked(Stream backingStore)
    {
      this.backingStore = backingStore;
    }

    public void Dispose()
    {
      ((IDisposable)backingStore).Dispose();
    }

    internal abstract Task ReadAll();
  }
}

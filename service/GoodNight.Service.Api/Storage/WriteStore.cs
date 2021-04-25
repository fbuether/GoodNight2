using GoodNight.Service.Domain.Model.Write;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Api.Storage
{
  public class WriteStore
  {
    internal IRepository<Quality> Qualities { get; }

    internal IRepository<Scene> Scenes { get; }

    internal IRepository<Story> Stories { get; }

    public WriteStore(IStore store)
    {
      Qualities = store.Create<Quality>("write-qualities");
      Scenes = store.Create<Scene>("write-scenes");
      Stories = store.Create<Story>("write-stories");
    }
  }
}

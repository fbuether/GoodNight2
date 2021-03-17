using GoodNight.Service.Domain.Model.Write;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Api.Storage
{
  public class WriteStore
  {
    internal IRepository<Quality, string> Qualities { get; }

    internal IRepository<Scene, string> Scenes { get; }

    internal IRepository<Story, string> Stories { get; }

    public WriteStore(IStore store)
    {
      Qualities = store.Create<Quality, string>("write-qualities");
      Scenes = store.Create<Scene, string>("write-scenes");
      Stories = store.Create<Story, string>("write-stories");
    }
  }
}

using GoodNight.Service.Domain.Model;
using GoodNight.Service.Domain.Model.Read;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Api.Storage
{
  public class ReadStore
  {
    internal IRepository<Adventure> Adventures { get; }

    internal IRepository<Log> Logs { get; }

    internal IRepository<Quality> Qualities { get; }

    internal IRepository<Scene> Scenes { get; }

    internal IRepository<Story> Stories { get; }

    internal IRepository<User> Users { get; }

    public ReadStore(IStore store)
    {
      Adventures = store.Create<Adventure>("read-adventures");
      Logs = store.Create<Log>("read-logs");
      Qualities = store.Create<Quality>("read-qualities");
      Scenes = store.Create<Scene>("read-scenes");
      Stories = store.Create<Story>("read-stories");
      Users = store.Create<User>("users");
    }
  }
}

using GoodNight.Service.Domain.Model;
using GoodNight.Service.Domain.Model.Read;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Api.Storage
{
  public class ReadStore
  {
    internal IRepository<Action, string> Actions { get; }

    internal IRepository<Quality, string> Qualities { get; }

    internal IRepository<Scene, string> Scenes { get; }

    internal IRepository<Story, string> Stories { get; }

    internal IRepository<User, string> Users { get; }

    public ReadStore(IStore store)
    {
      Actions = store.Create<Action, string>("actions");
      Qualities = store.Create<Quality, string>("qualities");
      Scenes = store.Create<Scene, string>("scenes");
      Stories = store.Create<Story, string>("stories");
      Users = store.Create<User, string>("users");
    }
  }
}

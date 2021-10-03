using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Model.Write.Transfer
{
  public record Reference(
    string Key,
    string? Name)
  {
    public Reference(IReference<Write.Scene> scene)
      : this(scene.Key, scene.Get()?.Name)
    {
    }

    public Reference(IReference<Write.Quality> quality)
      : this(quality.Key, quality.Get()?.Name)
    {
    }
  }
}

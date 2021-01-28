using System.Linq;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using GoodNight.Service.Domain.Write;
using GoodNight.Service.Store;

namespace GoodNight.Service.Api.Write
{
  [ApiController]
  [Route("api/v1/write/scenes")]
  public class WriteSceneController
  {
    private IRepository<Scene> repos;

    public WriteSceneController(IRepository<Scene> repos)
    {
      System.Console.WriteLine(repos);
      this.repos = repos;
    }


    [HttpGet]
    public ActionResult<IImmutableList<Scene>> Get()
    {
      var s = this.repos.Get().Where(s => s.Name == "FirstScene").FirstOrDefault();

      return new StatusCodeResult(StatusCodes.Status418ImATeapot);
    }
  }
}

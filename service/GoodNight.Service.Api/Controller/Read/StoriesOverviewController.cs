using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using GoodNight.Service.Domain.Model.Read;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Api.Controller.Read
{
  [ApiController]
  [Route("api/v1/read/stories")]
  public class StoriesOverviewController : ControllerBase
  {
    private IRepository<Story> stories;

    public StoriesOverviewController(IStore store)
    {
      stories = store.Create<Story>();
    }

    [HttpGet]
    public ActionResult<IEnumerable<Story>> GetAll()
    {
      return Ok(stories);
    }
  }
}

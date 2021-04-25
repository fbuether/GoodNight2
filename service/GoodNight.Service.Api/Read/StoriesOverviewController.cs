using System.Collections.Generic;
using GoodNight.Service.Api.Storage;
using GoodNight.Service.Domain.Model.Read;
using GoodNight.Service.Storage.Interface;
using Microsoft.AspNetCore.Mvc;

namespace GoodNight.Service.Api.Read
{
  [ApiController]
  [Route("api/v1/read/stories")]
  public class StoriesOverviewController : ControllerBase
  {
    private IRepository<Story> stories;

    public StoriesOverviewController(ReadStore store)
    {
      stories = store.Stories;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Story>> GetAll()
    {
      return Ok(stories);
    }
  }
}

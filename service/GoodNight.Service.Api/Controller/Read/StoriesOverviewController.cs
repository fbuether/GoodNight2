using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using GoodNight.Service.Domain.Model.Read;
using GoodNight.Service.Storage.Interface;
using TransferStory = GoodNight.Service.Domain.Model.Read.Transfer.Story;
using System.Linq;

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
    public ActionResult<IEnumerable<TransferStory>> GetAll()
    {
      return Ok(stories.Where(s => s.Public).Select(s => s.ToTransfer()));
    }

    [HttpGet("{storyUrlname}")]
    public ActionResult<TransferStory> Get(string storyUrlname)
    {
      var story = stories.Get(storyUrlname);
      if (story is null)
        return NotFound();

      return Ok(story.ToTransfer());
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using GoodNight.Service.Api.Controller.Base;
using GoodNight.Service.Domain.Model.Read;
using GoodNight.Service.Storage.Interface;
using Microsoft.AspNetCore.Mvc;
using TransferStory = GoodNight.Service.Domain.Model.Read.Transfer.Story;

namespace GoodNight.Service.Api.Controller.Read
{
  [ApiController]
  [Route("api/v1/read/user/stories")]
  public class ReadUserStoriesController : AuthorisedController
  {
    private IRepository<Story> stories;

    public ReadUserStoriesController(IStore store)
      : base(store)
    {
      stories = store.Create<Story>();
    }

    [HttpGet]
    public ActionResult<IEnumerable<TransferStory>> GetAllStarted()
    {
      var stories = GetCurrentUser().Adventures
        .Select(adv => adv.Get()?.Story.Get())
        .OfType<Story>()
        .OrderBy(s => s.Name)
        .Select(s => s.ToTransfer());

      return Ok(stories);
    }
  }
}

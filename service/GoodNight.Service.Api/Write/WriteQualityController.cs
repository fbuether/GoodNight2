using System.Collections.Generic;
using System.Linq;
using GoodNight.Service.Api.Storage;
using GoodNight.Service.Domain.Model.Write;
using GoodNight.Service.Domain.Util;
using GoodNight.Service.Storage.Interface;
using Microsoft.AspNetCore.Mvc;

namespace GoodNight.Service.Api.Write
{
  [ApiController]
  [Route("api/v1/write/stories/{storyUrlname}/qualities")]
  public class WriteQualityController : ControllerBase
  {
    private IRepository<Story> stories;

    private IRepository<Quality> qualities;

    public WriteQualityController(WriteStore repos)
    {
      stories = repos.Stories;
      qualities = repos.Qualities;
    }


    [HttpGet("{qualityUrlname}")]
    public ActionResult<IEnumerable<Quality>> Get(string storyUrlname,
      string qualityUrlname)
    {
      var quality = stories.FirstOrDefault(s => s.Urlname == storyUrlname)
        ?.GetQuality(qualityUrlname)
        ?.Get();
      return quality is not null
        ? Ok(quality)
        : NotFound();
    }


    public record RawQuality(string text);

    [HttpPost]
    public ActionResult<Quality> Create(string storyUrlname,
      [FromBody] RawQuality content)
    {
      var story = stories.FirstOrDefault(s => s.Urlname == storyUrlname);
      if (story is null)
        return NotFound();

      return Quality.Parse(content.text)
        .Map(story.SeizeQuality)
        .Map(qualities.Add)
        .Bind(q => Result.FailOnNull(q, "The quality already exists."))
        .Map(story.InsertNewQuality)
        .Map(sq => sq.MapFirst(stories.Save).Item2)
        .Map<ActionResult<Quality>>(quality => Accepted(
            $"api/v1/write/stories/{storyUrlname}/qualities/{quality.Key}",
            quality))
        .GetOrError(err => BadRequest(new ErrorResult(err)));
    }

    [HttpPut("{qualityUrlname}")]
    public ActionResult<Quality> Update(string storyUrlname,
      string qualityUrlname, [FromBody] RawQuality content)
    {
      var story = stories.FirstOrDefault(s => s.Urlname == storyUrlname);
      if (story is null)
        return NotFound();

      return Quality.Parse(content.text)
        .Filter(q => q.Urlname == qualityUrlname,
          "Qualities may not change name.")
        .Map(story.SeizeQuality)
        .Map(qualities.Update)
        .Bind(q => Result.FailOnNull(q, "The quality does not exist."))
        .Map<ActionResult<Quality>>(quality => Accepted(
            $"api/v1/write/stories/{storyUrlname}/qualities/{quality.Key}",
            quality))
        .GetOrError(err => BadRequest(new ErrorResult(err)));
    }
  }
}

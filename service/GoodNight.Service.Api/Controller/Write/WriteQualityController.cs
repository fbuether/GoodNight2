using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using GoodNight.Service.Domain.Model.Write;
using GoodNight.Service.Domain.Util;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Api.Controller.Write
{
  [ApiController]
  [Route("api/v1/write/stories/{storyUrlname}/qualities")]
  public class WriteQualityController : ControllerBase
  {
    private IRepository<Story> stories;

    private IRepository<Quality> qualities;

    public WriteQualityController(IStore store)
    {
      stories = store.Create<Story>();
      qualities = store.Create<Quality>();
    }


    [HttpGet("{qualityUrlname}")]
    public ActionResult<IEnumerable<Quality>> Get(string storyUrlname,
      string qualityUrlname)
    {
      var quality = stories.FirstOrDefault(s => s.Urlname == storyUrlname)
        ?.GetQuality(qualityUrlname);
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
        .Map(story.AddQuality)
        .Do(sq => stories.Save(sq.Item1))
        .Map(sq => sq.Item2)
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
        .Map(q => q with {Story = story.Urlname})
        .Map(qualities.Update)
        .Bind(q => Result.FailOnNull(q, "The quality does not exist."))
        .Map<ActionResult<Quality>>(quality => Accepted(
            $"api/v1/write/stories/{storyUrlname}/qualities/{quality.Key}",
            quality.Get()))
        .GetOrError(err => BadRequest(new ErrorResult(err)));
    }
  }
}

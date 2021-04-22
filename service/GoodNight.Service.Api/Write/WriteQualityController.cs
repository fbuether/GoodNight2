using System.Collections.Generic;
using System.Linq;
using GoodNight.Service.Api.Storage;
using GoodNight.Service.Domain.Model.Write;
using GoodNight.Service.Storage.Interface;
using Microsoft.AspNetCore.Mvc;

namespace GoodNight.Service.Api.Write
{
  [ApiController]
  [Route("api/v1/write/stories/{storyUrlname}/qualities")]
  public class WriteQualityController : ControllerBase
  {
    private IRepository<Story, string> stories;

    private IRepository<Quality, string> qualities;

    public WriteQualityController(WriteStore repos)
    {
      stories = repos.Stories;
      qualities = repos.Qualities;
    }


    [HttpGet("{qualityUrlname}")]
    public ActionResult<IEnumerable<Quality>> Get(string storyUrlname,
      string qualityUrlname)
    {
      var story = stories.FirstOrDefault(s => s.Urlname == storyUrlname);
      if (story is null)
        return NotFound();

      var quality = story.Qualities.FirstOrDefault(
        s => s.Urlname == qualityUrlname);
      if (quality is null)
        return NotFound();

      return Ok(quality);
    }


    public record RawQuality(string text);

    [HttpPost]
    public ActionResult<Quality> Create(string storyUrlname,
      [FromBody] RawQuality content)
    {
      var story = stories.FirstOrDefault(s => s.Urlname == storyUrlname);
      if (story is null)
        return NotFound();

      return story.AddNewQuality(content.text)
        .Map<ActionResult<Quality>>(
          (storyQuality) => {
            var (story, quality) = storyQuality;
            stories.Update(storyUrlname, (_) => story);
            return Created(
              $"api/v1/write/stories/{storyUrlname}/qualitys/{quality.Urlname}",
              quality);
          },
          err => BadRequest(new ErrorResult(err)));
    }

    [HttpPut("{qualityUrlname}")]
    public ActionResult<Quality> Update(string storyUrlname,
      string qualityUrlname, [FromBody] RawQuality content)
    {
      var story = stories.FirstOrDefault(s => s.Urlname == storyUrlname);
      if (story is null)
        return NotFound();

      var quality = story.Qualities.FirstOrDefault(s => s.Urlname == qualityUrlname);
      if (quality is null)
        return NotFound();

      return story.EditQuality(quality, content.text)
        .Map<ActionResult<Quality>>(
          (storyQuality) => {
            var (story, quality) = storyQuality;
            stories.Update(storyUrlname, (_) => story);
            return Accepted(
              $"api/v1/write/stories/{storyUrlname}/qualitys/{quality.Urlname}",
              quality);
          },
          err => BadRequest(new ErrorResult(err)));
    }
  }
}

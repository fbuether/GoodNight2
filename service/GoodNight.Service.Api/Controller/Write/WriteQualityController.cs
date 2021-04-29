using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using GoodNight.Service.Domain.Model.Write;
using GoodNight.Service.Domain.Util;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Domain.Parse;

namespace GoodNight.Service.Api.Controller.Write
{
  [ApiController]
  [Route("api/v1/write/stories/{storyUrlname}/qualities")]
  public class WriteQualityController : ControllerBase
  {
    private IRepository<Story> stories;
    private IRepository<Quality> qualities;
    private IRepository<Scene> scenes;
    private IRepository<Domain.Model.Read.Scene> readScenes;
    private IRepository<Domain.Model.Read.Story> readStories;

    public WriteQualityController(IStore store)
    {
      stories = store.Create<Story>();
      qualities = store.Create<Quality>();
      scenes = store.Create<Scene>();
      readScenes = store.Create<Domain.Model.Read.Scene>();
      readStories = store.Create<Domain.Model.Read.Story>();
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

      var readStory = readStories.First(s => s.Urlname == story.Urlname);

      var parsed = QualityParser.Parse(content.text)
        .ToResult();

      var readQuality = parsed
        .Bind(parsed => Domain.Model.Read.QualityFactory
          .Build(readScenes, parsed, story.Urlname))
        .Map(readStory.AddQuality);

      var writeQuality = parsed
        .Bind(parsed => QualityFactory.Build(
            parsed, content.text, story.Urlname))
        .Map(story.AddQuality)
        .Assure(sq => qualities.Get(sq.Item2.Key) is null,
          "A quality of this name already exists.");

      return writeQuality.And(readQuality)
        .Do(wsqrsq => stories.Save(wsqrsq.Item1.Item1))
        .Do(wsqrsq => readStories.Save(wsqrsq.Item2))
        .Map(wr => wr.Item1.Item2)
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

      var readStory = readStories.First(s => s.Urlname == story.Urlname);

      var parsed = QualityParser.Parse(content.text)
        .ToResult();

      var readQuality = parsed
        .Bind(parsed => Domain.Model.Read.QualityFactory
          .Build(readScenes, parsed, story.Urlname))
        .Map(readStory.AddQuality);

      var writeQuality = parsed
        .Bind(parsed => QualityFactory.Build(
            parsed, content.text, story.Urlname))
        .Map(story.AddQuality)
        .Assure(sq => qualities.Get(sq.Item2.Key) is not null,
          "The quality does not exist.")
        .Assure(sq => sq.Item2.Urlname == qualityUrlname,
          "Qualities may not change their name. Create a new quality.");

      return writeQuality.And(readQuality)
        .Do(wsqrsq => stories.Save(wsqrsq.Item1.Item1))
        .Do(wsqrsq => readStories.Save(wsqrsq.Item2))
        .Map(wr => wr.Item1.Item2)
        .Map<ActionResult<Quality>>(quality => Accepted(
            $"api/v1/write/stories/{storyUrlname}/qualities/{quality.Key}",
            quality))
        .GetOrError(err => BadRequest(new ErrorResult(err)));
    }
  }
}

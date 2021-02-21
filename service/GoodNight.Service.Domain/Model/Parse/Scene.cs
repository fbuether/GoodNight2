using System.Linq;
using System;
using System.Collections.Immutable;
using ModelScene = GoodNight.Service.Domain.Model.Write.Scene;
using System.Collections.Generic;

namespace GoodNight.Service.Domain.Model.Parse
{
  using ModelContent = Content<string, string>;

  public record Scene(
    string Raw,
    IImmutableList<Content> Content)
  {

    private ModelScene AddProps(ModelScene model, Content content)
    {
      // todo: warn on possibly overriding content?
      switch (content)
      {
        case Content.Name name:
          return model with { Name = name.DisplayName };
        case Content.IsStart _:
          return model with { IsStart = true };
        case Content.ShowAlways _:
          return model with { ShowAlways = true };
        case Content.ForceShow _:
          return model with { ForceShow = true };
        case Content.Tag tag:
          return model with { Tags = model.Tags.Add(tag.TagName) };
        case Content.Category category:
          return model with { Category = category.Path };
        case Content.Set setContent:
          return model with { Sets =
              model.Sets.Add((setContent.Quality, setContent.Expression)) };
        case Content.Return returnContent:
          return model with { Return = returnContent.Scene };
        case Content.Continue continueContent:
          return model with { Continue = continueContent.Scene };
        default:
          return model;
      }
    }

    private IImmutableList<ModelContent> TransformContent(
      IEnumerable<Content> content) =>
      content.Aggregate<Content, IImmutableList<ModelContent>>(
        ImmutableList<ModelContent>.Empty, AddContent);

    private IImmutableList<ModelContent> AddContent(
      IImmutableList<ModelContent> modelContent, Content content)
    {
      switch (content)
      {
        case Content.Text text:
          if (modelContent.Last() is ModelContent.Text<string, string> lastText)
          {
            return
              ImmutableList.CreateRange(
                modelContent
                .Take(modelContent.Count - 1)
                .Append(new ModelContent.Text<string, string>(
                    lastText.Value + "\n" + text.Value)));
          }
          else {
            return modelContent.Add(
              new ModelContent.Text<string, string>(text.Value));
          }

        case Content.Require require:
          return modelContent.Add(
            new ModelContent.Require<string, string>(require.Expression));

        case Content.Option option:
          return modelContent.Add(
            new ModelContent.Option<string, string>(option.Scene,
              TransformContent(option.Content)));

        case Content.Condition cond:
          return modelContent.Add(new ModelContent.Condition<string, string>(
              cond.If,
              TransformContent(cond.Then),
              TransformContent(cond.Else)));

        case Content.Include include:
          return modelContent.Add(
            new ModelContent.Include<string, string>(include.Scene));

        default:
          // todo: warn about possibly unused content?
          return modelContent;
      }
    }


    public ModelScene ToModel()
    {
      var model = ModelScene.CreateDefault() with { Raw = Raw };
      model = model with { Content =
        Content.Aggregate(model.Content, AddContent) };
      return Content.Aggregate(model, AddProps);
    }
  }
}

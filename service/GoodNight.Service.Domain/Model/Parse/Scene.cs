using System.Linq;
using System;
using System.Collections.Immutable;
using WriteScene = GoodNight.Service.Domain.Model.Write.Scene;
using WriteContent = GoodNight.Service.Domain.Model.Write.Content;
using System.Collections.Generic;

namespace GoodNight.Service.Domain.Model.Parse
{

  public record Scene(
    string Raw,
    IImmutableList<Content> Content)
  {
    public static Scene Empty
    {
      get
      {
        return new Scene("", ImmutableList.Create<Content>());
      }
    }

    private WriteScene AddProps(WriteScene model, Content content)
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

    private IImmutableList<WriteContent> TransformContent(
      IEnumerable<Content> content) =>
      content.Aggregate<Content, IImmutableList<WriteContent>>(
        ImmutableList<WriteContent>.Empty, AddContent);

    private IImmutableList<WriteContent> AddContent(
      IImmutableList<WriteContent> modelContent, Content content)
    {
      switch (content)
      {
        case Content.Text text:
          if (modelContent.Any() &&
            modelContent.Last() is WriteContent.Text lastText)
          {
            return
              ImmutableList.CreateRange(
                modelContent
                .Take(modelContent.Count - 1)
                .Append(new WriteContent.Text(
                    lastText.Value + "\n" + text.Value)));
          }
          else {
            return modelContent.Add(
              new WriteContent.Text(text.Value));
          }

        case Content.Require require:
          return modelContent.Add(
            new WriteContent.Require(require.Expression));

        case Content.Option option:
          return modelContent.Add(
            new WriteContent.Option(option.Scene,
              TransformContent(option.Content)));

        case Content.Condition cond:
          return modelContent.Add(new WriteContent.Condition(
              cond.If,
              TransformContent(cond.Then),
              TransformContent(cond.Else)));

        case Content.Include include:
          return modelContent.Add(
            new WriteContent.Include(include.Scene));

        default:
          // todo: warn about possibly unused content?
          return modelContent;
      }
    }

    public Scene AddContent(Content newContent)
    {
      return this with { Content = Content.Add(newContent) };
    }

    public WriteScene ToModel()
    {
      var model = WriteScene.Empty with { Raw = Raw };
      var writeContent = Content.Aggregate(model.Content, AddContent);
      return Content.Aggregate(model with { Content = writeContent }, AddProps);
    }
  }
}

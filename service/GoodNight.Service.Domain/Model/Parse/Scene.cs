using System;
using System.Linq;
using System.Collections.Immutable;
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

    private Write.Scene AddProps(Write.Scene model, Content content)
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

    private IImmutableList<Write.Content> TransformContent(
      IEnumerable<Content> content) =>
      content.Aggregate<Content, IImmutableList<Write.Content>>(
        ImmutableList<Write.Content>.Empty, AddContent);

    private IImmutableList<Write.Content> AddContent(
      IImmutableList<Write.Content> modelContent, Content content)
    {
      switch (content)
      {
        case Content.Text text:
          if (modelContent.Any() &&
            modelContent.Last() is Write.Content.Text lastText)
          {
            return
              ImmutableList.CreateRange(
                modelContent
                .Take(modelContent.Count - 1)
                .Append(new Write.Content.Text(
                    lastText.Value + "\n" + text.Value)));
          }
          else {
            return modelContent.Add(
              new Write.Content.Text(text.Value));
          }

        case Content.Require require:
          return modelContent.Add(
            new Write.Content.Require(require.Expression));

        case Content.Option option:
          return modelContent.Add(
            new Write.Content.Option(option.Scene,
              TransformContent(option.Content)));

        case Content.Condition cond:
          return modelContent.Add(new Write.Content.Condition(
              cond.If,
              TransformContent(cond.Then),
              TransformContent(cond.Else)));

        case Content.Include include:
          return modelContent.Add(
            new Write.Content.Include(include.Scene));

        default:
          // todo: warn about possibly unused content?
          return modelContent;
      }
    }

    public Scene AddContent(Content newContent)
    {
      return this with { Content = Content.Add(newContent) };
    }

    public Write.Scene ToWriteModel()
    {
      var model = Write.Scene.Empty with { Raw = Raw };
      var contents = Content.Aggregate(model.Contents, AddContent);
      return Content.Aggregate(model with { Contents = contents }, AddProps);
    }

    public Read.Scene ToReadModel()
    {
      throw new NotImplementedException();
    }
  }
}

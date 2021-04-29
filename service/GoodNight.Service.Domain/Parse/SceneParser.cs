using Pidgin;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections.Immutable;
using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Domain.Model.Parse;

namespace GoodNight.Service.Domain.Parse
{
  using ContentParser = Parser<char, Scene.Content>;
  using ContentListParser = Parser<char, IEnumerable<Scene.Content>>;

  public static class SceneParser
  {
    private readonly static ContentParser nameContent =
      Parser.Try(Parser.String("name"))
      .Then(NameParser.Colon)
      .Then(NameParser.SceneName)
      .Map<Scene.Content>(name => new Scene.Content.Name(name.Trim()));

    private readonly static ContentParser isStartContent =
      Parser.Try(Parser.String("start"))
      .Map<Scene.Content>(_ => new Scene.Content.IsStart());

    private readonly static ContentParser showAlwaysContent =
      Parser.Try(Parser.String("show"))
      .Then(NameParser.InlineWhitespace)
      .Then(Parser.String("always"))
      .Or(Parser.String("always")
        .Then(NameParser.InlineWhitespace)
        .Then(Parser.String("show")))
      .Map<Scene.Content>(_ => new Scene.Content.ShowAlways());

    private readonly static ContentParser forceShowContent =
      Parser.String("force")
      .Then(NameParser.InlineWhitespace)
      .Then(Parser.String("show"))
      .Map<Scene.Content>(_ => new Scene.Content.ForceShow());

    private readonly static ContentListParser tagsContent =
      Parser.String("tag")
      .Then(Parser.Char('s').Optional())
      .Then(NameParser.Colon)
      .Then(NameParser.TagName.Separated(Parser.Char(',')))
      .Map<IEnumerable<Scene.Content>>(tags =>
        tags.Select(t =>
          new Scene.Content.Tag(t.Trim())));

    private readonly static ContentParser categoryContent =
      Parser.Try(Parser.String("cat")
        .Then(Parser.String("egory").Optional()))
      .Then(NameParser.Colon)
      .Then(NameParser.CategoryName.Separated(Parser.Char('/')))
      .Map<Scene.Content>(cs => new Scene.Content.Category(
          ImmutableArray.Create<string>(cs.ToArray())));


    private readonly static ContentParser setContent =
      Parser.String("set")
      .Then(NameParser.Colon)
      .Then(NameParser.QualityName)
      .Before(NameParser.InlineWhitespace
        .Then(Parser.Char('='))
        .Then(NameParser.InlineWhitespace)).
      Then<Expression<string>, Scene.Content>(ExpressionParser.Expression,
        (quality,expr) => new Scene.Content.Set(quality, expr));

    private readonly static ContentParser requireContent =
      Parser.Try(Parser.String("require"))
      .Then(NameParser.Colon)
      .Then(ExpressionParser.Expression)
      .Map<Scene.Content>(expr => new Scene.Content.Require(expr));


    private readonly static ContentParser nextSceneContent =
      Parser.OneOf(
        Parser.Try(Parser.String("return"))
        .WithResult(typeof(Scene.Content.Return)),
        Parser.Try(Parser.String("include"))
        .WithResult(typeof(Scene.Content.Include)),
        Parser.Try(Parser.String("continue"))
        .WithResult(typeof(Scene.Content.Continue)))
      .Before(NameParser.Colon)
      .Then<string, Scene.Content>(NameParser.SceneName,
        (T, scene) => (Scene.Content)Activator.CreateInstance(T, scene)!);



    private readonly static Parser<char, Expression<string>> conditionIf =
      Parser.Try(Parser.String("if"))
      .Then(NameParser.Colon)
      .Then(ExpressionParser.Expression)
      .Before(NameParser.InlineWhitespace)
      .Before(Parser.EndOfLine);

    private readonly static Parser<char, Unit> conditionElse =
      Parser.Char('$')
      .Then(NameParser.InlineWhitespace)
      .Then(Parser.String("else"))
      .Then(NameParser.InlineWhitespace)
      .Then(Parser.EndOfLine)
      .Map(_ => Unit.Value);

    private readonly static Parser<char, Unit> endContent =
      Parser.Char('$')
      .Then(NameParser.InlineWhitespace)
      .Then(Parser.String("end"))
      .Then(NameParser.InlineWhitespace)
      .Map(_ => Unit.Value);

    private static ContentParser conditionalContent(
      ContentListParser parseLines) =>
      conditionIf
      .Then(parseLines,
        (Expression<string> cond, IEnumerable<Scene.Content> then) => (cond, then))
      .Then<Maybe<IEnumerable<Scene.Content>>, Scene.Content>(
        Parser.Try(
          conditionElse
          .Then(parseLines)
        ).Optional()
        .Before(endContent),

        ((Expression<string>, IEnumerable<Scene.Content>) condThen,
          Maybe<IEnumerable<Scene.Content>> elseContent) =>
        new Scene.Content.Condition(condThen.Item1,
          ImmutableArray.Create<Scene.Content>(condThen.Item2.ToArray()),
          ImmutableArray.Create<Scene.Content>(
            elseContent
            .Select(elseContent => elseContent.ToArray())
            .GetValueOrDefault(new Scene.Content[] { }))));


    private static ContentParser optionContent(
      ContentListParser parseLines) =>
      Parser.Try(Parser.String("option"))
      .Then(NameParser.Colon)
      .Then(NameParser.SceneName)
      .Before(NameParser.InlineWhitespace)
      .Before(Parser.EndOfLine)
      .Then<IEnumerable<Scene.Content>, Scene.Content>(
        parseLines
        .Before(endContent),
        (string sceneName, IEnumerable<Scene.Content> optionContent) =>
        new Scene.Content.Option(sceneName,
          ImmutableArray.Create<Scene.Content>(optionContent.ToArray())));



    private static ContentListParser ToList<T>(Parser<char, T> parser)
      where T : Scene.Content =>
      parser.Map<IEnumerable<Scene.Content>>(content => new[] { content });


    private static ContentListParser settingContent(
      ContentListParser parseLines) =>
      Parser.Char('$')
      .Then(NameParser.InlineWhitespace)
      .Then(Parser.OneOf(
          ToList(nameContent),
          ToList(isStartContent),
          ToList(showAlwaysContent),
          ToList(forceShowContent),
          tagsContent.Map<IEnumerable<Scene.Content>>(contents => contents.ToArray()),
          ToList(categoryContent),
          ToList(setContent),
          ToList(requireContent),
          ToList(nextSceneContent),
          ToList(optionContent(parseLines)),
          ToList(conditionalContent(parseLines))
        ))
      .Before(NameParser.InlineWhitespace)
      .Select<IEnumerable<Scene.Content>>(c => c);

    private readonly static Parser<char, Scene.Content.Text> textContent =
      Parser.AnyCharExcept("$\r\n") // not EOL or setting starting with $
      .Then(NameParser.RemainingLine, (lead, tail) => lead + tail)
      .Map(text => new Scene.Content.Text(text));

    private readonly static ContentListParser emptyLine =
      ToList(Parser.Lookahead(Parser.EndOfLine)
        .WithResult<Scene.Content>(new Scene.Content.Text("")));


    private static ContentListParser parseLine(ContentListParser parseLines) =>
      Parser.Try(settingContent(parseLines))
      .Or(Parser.Try(ToList(textContent)))
      .Or(emptyLine);


    private static ContentListParser parseLines(ContentListParser self) =>
      Parser.Try(parseLine(self))
      .SeparatedAndOptionallyTerminated(Parser.EndOfLine)
      // flatten due to double wraping of settings
      .Map<IEnumerable<Scene.Content>>(c => c.SelectMany(c => c).ToArray());


    private readonly static ContentListParser parseLinesRec =
      Parser.Rec<char, IEnumerable<Scene.Content>>(parseLines);


    public static ParseResult<Scene> Parse(string content)
    {
      var res = parseLinesRec
        .Before(Parser<char>.End)
        .Parse(content);

      return res.Success
        ? new ParseResult.Success<Scene>(
          new Scene(content,
            ImmutableArray.Create<Scene.Content>(res.Value.ToArray())))
        : ParseResult.Failure<Scene>.OfError(res.Error);
    }
  }
}

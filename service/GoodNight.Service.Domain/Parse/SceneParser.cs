using Pidgin;
using GoodNight.Service.Domain.Write;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections.Immutable;
using GoodNight.Service.Domain.Write.Expressions;

namespace GoodNight.Service.Domain.Parse
{
  using ContentParser = Parser<char, IEnumerable<Content>>;

  public class SceneParser
  {
    private readonly static Parser<char, Unit> colon =
      NameParser.InlineWhitespace
      .Then(Parser.Char(':'))
      .Then(NameParser.InlineWhitespace)
      .WithResult(Unit.Value);

    private readonly static Parser<char, string> remainingLine =
      Parser.AnyCharExcept("\r\n").ManyString();


    private readonly static Parser<char, Content> nameContent =
      Parser.Try(Parser.String("name"))
      .Then(colon)
      .Then(NameParser.SceneName)
      .Map<Content>(name => new Content.Name(name.Trim()));

    private readonly static Parser<char, Content> isStartContent =
      Parser.Try(Parser.String("start"))
      .Map<Content>(_ => new Content.IsStart());

    private readonly static Parser<char, Content> showAlwaysContent =
      Parser.Try(Parser.String("show"))
        .Then(NameParser.InlineWhitespace)
        .Then(Parser.String("always"))
      .Or(Parser.String("always")
        .Then(NameParser.InlineWhitespace)
        .Then(Parser.String("show")))
      .Map<Content>(_ => new Content.ShowAlways());

    private readonly static Parser<char, Content> forceShowContent =
      Parser.String("force")
      .Then(NameParser.InlineWhitespace)
      .Then(Parser.String("show"))
      .Map<Content>(_ => new Content.ForceShow());

    private readonly static ContentParser tagsContent =
      Parser.String("tag")
      .Then(Parser.Char('s').Optional())
      .Then(colon)
      .Then(NameParser.TagName.Separated(Parser.Char(',')))
      .Map<IEnumerable<Content>>(tags =>
        tags.Select(t =>
          new Content.Tag(t.Trim())));

    private readonly static Parser<char, Content> categoryContent =
      Parser.Try(Parser.String("cat")
        .Then(Parser.String("egory").Optional()))
      .Then(colon)
      .Then(NameParser.CategoryName.Separated(Parser.Char('/')))
      .Map<Content>(cs => new Content.Category(
          ImmutableArray.Create<string>(cs.ToArray())));


    private readonly static Parser<char, Content> setContent =
      Parser.String("set")
      .Then(colon)
      .Then(NameParser.QualityName)
      .Before(NameParser.InlineWhitespace
        .Then(Parser.Char('='))
        .Then(NameParser.InlineWhitespace)).
      Then<Expression, Content>(ExpressionParser.expression, (quality,expr) =>
        new Content.Set(quality, expr));

    private readonly static Parser<char, Content> requireContent =
      Parser.Try(Parser.String("require"))
      .Then(colon)
      .Then(ExpressionParser.expression)
      .Map<Content>(expr => new Content.Require(expr));


    private readonly static Parser<char, Content> nextSceneContent =
      Parser.OneOf(
        Parser.Try(Parser.String("return")).WithResult(typeof(Content.Return)),
        Parser.Try(Parser.String("include")).WithResult(typeof(Content.Include)),
        Parser.Try(Parser.String("continue")).WithResult(typeof(Content.Continue)))
      .Before(colon)
      .Then<string, Content>(NameParser.SceneName,
        (T, scene) => (Content)Activator.CreateInstance(T, scene)!);



    private readonly static Parser<char, Expression> conditionIf =
      Parser.Try(Parser.String("if"))
      .Then(colon)
      .Then(ExpressionParser.expression)
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

    private static Parser<char, Content> conditionalContent(ContentParser parseLines) =>
      conditionIf
      .Then(parseLines,
        (Expression cond, IEnumerable<Content> then) => (cond, then))
      .Then<Maybe<IEnumerable<Content>>, Content>(
        Parser.Try(
          conditionElse
          .Then(parseLines)
        ).Optional()
        .Before(endContent),

          ((Expression, IEnumerable<Content>) condThen,
            Maybe<IEnumerable<Content>> elseContent) =>
            new Content.Condition(condThen.Item1,
              ImmutableArray.Create<Content>(condThen.Item2.ToArray()),
              ImmutableArray.Create<Content>(
                elseContent
                .Select(elseContent => elseContent.ToArray())
                .GetValueOrDefault(new Content[] { }))));


    private static Parser<char, Content> optionContent(ContentParser parseLines) =>
      Parser.Try(Parser.String("option"))
      .Then(colon)
      .Then(NameParser.SceneName)
      .Before(NameParser.InlineWhitespace)
      .Before(Parser.EndOfLine)
      .Then<IEnumerable<Content>, Content>(
        parseLines
        .Before(endContent),
        (string sceneName, IEnumerable<Content> optionContent) =>
        new Content.Option(sceneName,
          ImmutableArray.Create<Content>(optionContent.ToArray())));



    private static ContentParser ToList<T>(Parser<char, T> parser)
      where T : Content =>
      parser.Map<IEnumerable<Content>>(content => new[] { content });


    private static ContentParser settingContent(ContentParser parseLines) =>
      Parser.Char('$')
      .Then(NameParser.InlineWhitespace)
      .Then(Parser.OneOf(
          ToList(nameContent),
          ToList(isStartContent),
          ToList(showAlwaysContent),
          ToList(forceShowContent),
          tagsContent.Map<IEnumerable<Content>>(contents => contents.ToArray()),
          ToList(categoryContent),
          ToList(setContent),
          ToList(requireContent),
          ToList(nextSceneContent),
          ToList(optionContent(parseLines)),
          ToList(conditionalContent(parseLines))
        ))
      .Before(NameParser.InlineWhitespace)
      .Select<IEnumerable<Content>>(c => c);

    private readonly static Parser<char, Content.Text> textContent =
      Parser.AnyCharExcept("$\r\n") // not EOL or setting starting with $
      .Then(remainingLine, (lead, tail) => lead + tail)
      .Map(text => new Content.Text(text));

    private readonly static ContentParser emptyLine =
      ToList(Parser.Lookahead(Parser.EndOfLine)
        .WithResult<Content>(new Content.Text("")));


    private static ContentParser parseLine(ContentParser parseLines) =>
      Parser.Try(settingContent(parseLines))
      .Or(Parser.Try(ToList(textContent)))
      .Or(emptyLine);


    private static ContentParser parseLines(ContentParser self) =>
      Parser.Try(parseLine(self))
      .SeparatedAndOptionallyTerminated(Parser.EndOfLine)
      // flatten due to double wraping of settings
      .Map<IEnumerable<Content>>(c => c.SelectMany(c => c).ToArray());


    private readonly static ContentParser parseLinesRec =
      Parser.Rec<char, IEnumerable<Content>>(parseLines);


    public ParseResult<IImmutableList<Content>> Parse(string content)
    {
      var res = parseLinesRec
        .Before(Parser<char>.End)
        .Parse(content);

      return new ParseResult<IImmutableList<Content>>(res.Success,
        res.Success ? ImmutableArray.Create<Content>(res.Value.ToArray()) : null,
        !res.Success && res.Error is not null
          ? res.Error.Message
          : null,
        !res.Success && res.Error is not null
          ? new Tuple<int,int>(res.Error.ErrorPos.Line, res.Error.ErrorPos.Col)
          : null,
        !res.Success && res.Error is not null && res.Error.Unexpected.HasValue
          ? res.Error.Unexpected.Value.ToString()
          : null,
        !res.Success && res.Error is not null
          ? String.Join(", ", res.Error.Expected.Select(e => e.ToString()))
          : null);
    }
  }
}

using Pidgin;
using GoodNight.Service.Domain.Write;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections.Immutable;
using GoodNight.Service.Domain.Write.Expressions;

namespace GoodNight.Service.Domain.Parse
{
  public class SceneParser
  {

    // sort these.

    // Names of qualities are letters, digits, underscores and spaces if quoted.
    private readonly static Parser<char, string> qualityName =
      Parser.Not(Parser.Try(Parser.OneOf(
            // Keywords must not be valid qualitynames.
            Parser.String("true"),
            Parser.String("false"))))
      .Then(
        Parser.LetterOrDigit.Or(Parser.OneOf("_ ")).ManyString()
        .Between(Parser.Char('"'))
        .Or(Parser.LetterOrDigit.Or(Parser.Char('_')).ManyString()));


    // fin sort these.

    private readonly static Parser<char, Unit> inlineWhitespace =
      Parser.OneOf(" \t").SkipMany();

    private readonly static Parser<char, Unit> colon =
      inlineWhitespace.Then(Parser.Char(':')).Then(inlineWhitespace);

    private readonly static Parser<char, string> remainingLine =
      Parser.AnyCharExcept("\r\n").ManyString();

    private readonly static Parser<char, string> sceneName =
      Parser.AnyCharExcept("\r\n\":").ManyString();

    private readonly static Parser<char, string> tagName =
      Parser.AnyCharExcept("\r\n\":,").ManyString();

    private readonly static Parser<char, string> categoryName =
      Parser.AnyCharExcept("\r\n\":/").ManyString();


    private readonly static Parser<char, Content> nameContent =
      Parser.String("name")
      .Then(colon)
      .Then(sceneName)
      .Map<Content>(name => new Content.Name(name.Trim()));

    private readonly static Parser<char, Content> isStartContent =
      Parser.String("start")
      .Map<Content>(_ => new Content.IsStart());

    private readonly static Parser<char, Content> showAlwaysContent =
      Parser.Try(Parser.String("show")
        .Then(inlineWhitespace)
        .Then(Parser.String("always")))
      .Or(Parser.String("always")
        .Then(inlineWhitespace)
        .Then(Parser.String("show")))
      .Map<Content>(_ => new Content.ShowAlways());

    private readonly static Parser<char, Content> forceShowContent =
      Parser.String("force")
      .Then(inlineWhitespace)
      .Then(Parser.String("show"))
      .Map<Content>(_ => new Content.ForceShow());

    private readonly static Parser<char, IEnumerable<Content>> tagsContent =
      Parser.String("tag")
      .Then(Parser.Char('s').Optional())
      .Then(colon)
      .Then(tagName.Separated(Parser.Char(',')))
      .Map<IEnumerable<Content>>(tags => tags.Select(t => new Content.Tag(t)));

    private readonly static Parser<char, Content> categoryContent =
      Parser.String("cat")
      .Then(Parser.String("egory").Optional())
      .Then(colon)
      .Then(categoryName.Separated(Parser.Char('/')))
      .Map<Content>(cs => new Content.Category(
          ImmutableArray.Create<string>(cs.ToArray())));


    private readonly static Parser<char, Content> setContent =
      Parser.String("set")
      .Then(colon)
      .Then(
        Parser.Map<char, string, Expression, Content>((quality,expr) =>
          new Content.Set(quality, expr),
          qualityName
          .Before(inlineWhitespace
            .Then(Parser.Char('='))
            .Then(inlineWhitespace)),
          ExpressionParser.expression));

    private readonly static Parser<char, Content> requireContent =
      Parser.String("require")
      .Then(colon)
      .Then(ExpressionParser.expression)
      .Map<Content>(expr => new Content.Require(expr));


    private readonly static Parser<char, Content> nextSceneContent =
      Parser.Map<char, string, string, Content>((key, scene) => {
        switch(key) {
          case "option": return new Content.Option(scene);
          case "return": return new Content.Return(scene);
          case "continue": return new Content.Continue(scene);
          // cannot happen, because the first arg is just one of the keys.
          default: return new Content.Text("INVALID NEXT SCENE: " + scene);
        }
      },
        Parser.OneOf(Parser.String("option"),
          Parser.String("return"),
          Parser.String("contine"))
        .Before(colon),
        sceneName);


    private static IEnumerable<T> YieldSingle<T>(T single) {
      yield return single;
    }

    private static Parser<char, IEnumerable<T>> AsList<T>(
      Parser<char, T> parser) =>
      parser.Map<IEnumerable<T>>(YieldSingle);

    private readonly static Parser<char, IEnumerable<Content>> settingContent =
      Parser.Char('$')
      .Then(inlineWhitespace)
      .Then(Parser.OneOf(
          AsList(Parser.Try(nameContent)),
          AsList(Parser.Try(isStartContent)),
          AsList(Parser.Try(showAlwaysContent)),
          AsList(Parser.Try(forceShowContent)),
          Parser.Try(tagsContent),
          AsList(Parser.Try(categoryContent)),
          AsList(Parser.Try(setContent)),
          AsList(Parser.Try(requireContent)),
          AsList(Parser.Try(nextSceneContent))
        ))
      .Before(inlineWhitespace);

    private readonly static Parser<char, Content> textContent =
      Parser.Map(
        (first, remainder) => first + remainder,
        Parser.AnyCharExcept("$"),
        remainingLine)
      .Map<Content>(text => new Content.Text(text));


    private readonly static Parser<char, IEnumerable<Content>> parseLinesRec =
      Parser.Rec(() => parseLines!);

    private readonly static Parser<char, IEnumerable<Content>> parseLineRec =
      Parser.Rec(() => parseLine!);


    private readonly static Parser<char, Expression> conditionIf =
      Parser.Char('$')
      .Then(inlineWhitespace)
      .Then(Parser.String("if"))
      .Then(colon)
      .Then(ExpressionParser.expression)
      .Before(Parser.EndOfLine);

    private readonly static Parser<char, Unit> conditionElse =
      Parser.Char('$')
      .Then(inlineWhitespace)
      .Then(Parser.String("else"))
      .Then(remainingLine)
      .Then(Parser.EndOfLine)
      .Map(_ => Unit.Value);

    private readonly static Parser<char, Unit> conditionEnd =
      Parser.Char('$')
      .Then(inlineWhitespace)
      .Then(Parser.String("end"))
      .Then(remainingLine)
      .Map(_ => Unit.Value);

    private readonly static Parser<char, Content> conditionalContent =
      Parser.Map<char, Expression, IEnumerable<Content>,
      Maybe<IEnumerable<Content>>, Content>(
        (cond, thenExpr, maybeElseExpr) =>
          new Content.Condition(cond,
            ImmutableArray.Create<Content>(thenExpr.ToArray()),
            maybeElseExpr.Select(elseExpr =>
              ImmutableArray.Create<Content>(elseExpr.ToArray()))
            .GetValueOrDefault(
              ImmutableArray.Create<Content>())),

        conditionIf,

        Parser.Try(
          Parser.Not(Parser.Try(conditionElse))
          .Then(Parser.Not(Parser.Try(conditionEnd)))
          .Then(parseLineRec))
        .Separated(Parser.EndOfLine)
        // flatten due to double wraping of settings
        .Map<IEnumerable<Content>>(c => c.SelectMany(c => c)),

        Parser.Try(conditionElse
          .Then(parseLinesRec)
          .Before(Parser.EndOfLine))
          .Optional()
        .Before(conditionEnd));


    private readonly static Parser<char, IEnumerable<Content>> parseLine =
      Parser.Try(AsList(textContent))
      .Or(Parser.Try(settingContent))
      .Or(AsList(conditionalContent));

    private readonly static Parser<char, IEnumerable<Content>> parseLines =
      parseLine
      .Separated(Parser.EndOfLine)
      // flatten due to double wraping of settings
      .Map<IEnumerable<Content>>(c => c.SelectMany(c => c));



    // .Labelled("name")?


    public ParseResult<IImmutableList<Content>> Parse(string content)
    {
      var res = parseLines
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
          : null);
    }
  }
}

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

  /**
   * SceneParser transforms textual representation of Scenes into
   * Model.Parse.Scene objects. It recognises this structure:
   *
   * A scene is composed of a list of lines. A line may either start with a
   * $ symbol (no preceeding space allowed), or it can contain any arbitrary
   * text, which is considered as untransformed text.
   *
   * Lines starting with $ introduce behaviour into the scene. After the $, the
   * line contains a keyword and a colon (:). These behaviours are known:
   * - $name: <string>
   *   Names this scene to be refered by other scenes. The name cannot be
   *   changed after the scene has been saved the first time. Only one allowed.
   * - $start
   *   Denotes this scene as the first scene of a story.
   * - $show always / $always show
   *   Only valid in an option. Forces this option to be shown, even if the
   *   player does not fulfil the requirements.
   *   // todo: not implemented.
   * - $force show
   *   Forces this scene to be played as soon as a player can play this scene.
   *   // todo: not implemented.
   * - $tag: <string>[, <string>]*
   *   Adds a tag to this scene to be easily identified in editing mode. Has no
   *   effect during play.
   * - $cat[egory]: <string>
   *   Sets the category of this scene. Only one allowed. Can contain '/' to
   *   create nested categories.
   * - $set: <quality> = <expr>
   *   Sets quality <quality> of the player to the value of computing <expr>
   *   with the player's state as context.
   * - $require: [(<label>)] <expr>
   *   When used inside an option, requires <expr> to evaluate to true to make
   *   the option available. If used outside an option, does nothing. Optionally
   *   may have a label text in braces.
   * - $continue: <scene>
   *   If used directly in a scene, allows the player to continue to this scene.
   *   If used inside an option, denotes the scene that the option continues to.
   * - $return: <scene>
   *   If used directly in a scene, allows the player to return to this scene.
   *   Discarded if inside an option.
   * - $include: <scene>
   *   Includes the denoted scene verbatim here, and interprets it, ignoring
   *   the name. May transitively include other scenes.
   * - $if: <expr>
   *   [$else]
   *   $end
   *   Evaluates <expr>. If the result is true, interprets the part between
   *   if and else, otherwise the part between else and end (or nothing, if else
   *   is not present).
   * - $option
   *   $end
   *   Adds a new option to a scene. Must include at least a $continue to denote
   *   the scene to continue towards.
   */
  public static class SceneParser
  {
    private readonly static ContentParser nameContent =
      Parser.Try(Parser.String("name"))
      .Labelled("Name-Statement (begin with $name:)")
      .Then(NameParser.Colon)
      .Then(NameParser.SceneName)
      .Map<Scene.Content>(name => new Scene.Content.Name(name));

    private readonly static ContentParser isStartContent =
      Parser.Try(Parser.String("start"))
      .Map<Scene.Content>(_ => new Scene.Content.IsStart())
      .Labelled("Start-Statement ($start)");

    private readonly static ContentParser showAlwaysContent =
      Parser.Try(Parser.String("show"))
      .Then(NameParser.InlineWhitespace)
      .Then(Parser.String("always"))
      .Or(Parser.String("always")
        .Then(NameParser.InlineWhitespace)
        .Then(Parser.String("show")))
      .Map<Scene.Content>(_ => new Scene.Content.ShowAlways())
      .Labelled("ShowAlways-Statement ($show always/$always show)");

    private readonly static ContentParser forceShowContent =
      Parser.String("force")
      .Then(NameParser.InlineWhitespace)
      .Then(Parser.String("show"))
      .Map<Scene.Content>(_ => new Scene.Content.ForceShow())
      .Labelled("ForceShow-Statement ($force[ show])");

    private readonly static ContentParser tagsContent =
      Parser.Try(Parser.String("tag"))
      .Then(Parser.Char('s').Optional())
      .Labelled("Tags-Statement (begin with $tag[s]:)")
      .Then(NameParser.Colon)
      .Then(NameParser.TagName.Separated(Parser.Char(',')))
      .Map<Scene.Content>(tags =>
        new Scene.Content.Tag(
          ImmutableList.CreateRange(tags)));

    private readonly static ContentParser categoryContent =
      Parser.Try(Parser.String("cat")
        .Then(Parser.String("egory").Optional()))
      .Labelled("Category-Statement (begin with $cat[egory]:)")
      .Then(NameParser.Colon)
      .Then(NameParser.CategoryName.Separated(Parser.Char('/')))
      .Map<Scene.Content>(cs => new Scene.Content.Category(
          ImmutableArray.Create<string>(cs.ToArray())));


    private readonly static ContentParser setContent =
      Parser.String("set")
      .Then(NameParser.Colon)
      .Labelled("Set-Statement (begin with $set:)")
      .Then(NameParser.QualityName)
      .Before(NameParser.InlineWhitespace)
      .Then<(Scene.Content.SetOperator, Expression<string>), Scene.Content>(
        Parser.OneOf(new[] {
            Parser.String("=").WithResult(Scene.Content.SetOperator.Set),
            Parser.String("+=").WithResult(Scene.Content.SetOperator.Add),
            Parser.String("-=").WithResult(Scene.Content.SetOperator.Sub),
            Parser.String("*=").WithResult(Scene.Content.SetOperator.Mult),
            Parser.String("/=").WithResult(Scene.Content.SetOperator.Div) })
        .Labelled(@"Set-Operator (""="" with optionally one of ""+-*/"" in front)")
        .Before(NameParser.InlineWhitespace)
        .Then(ExpressionParser.Expression,
          (symbol, expr) => (symbol, expr)),
        (name, se) => new Scene.Content.Set(name, se.Item1, se.Item2));

    private readonly static ContentParser requireContent =
      Parser.Try(Parser.String("require"))
      .Then(NameParser.Colon)
      .Labelled("Require-Statement (begin with $require:)")
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
      .Labelled("Else-Statement (only $else)")
      .Map(_ => Unit.Value);

    private readonly static Parser<char, Unit> endContent =
      Parser.Char('$')
      .Then(NameParser.InlineWhitespace)
      .Then(Parser.String("end"))
      .Then(NameParser.InlineWhitespace)
      .Labelled("End-Statement (only $end)")
      .Map(_ => Unit.Value);

    private readonly static Parser<char, Unit> notElseOrEnd =
      Parser.Try(Parser.Not(Parser.OneOf(
            Parser.Try(conditionElse),
            Parser.Try(endContent))));

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
      .Before(NameParser.InlineWhitespace)
      .Before(Parser.EndOfLine)
      .Then(parseLines
        .Before(endContent))
      .Map<Scene.Content>(optionContent =>
        new Scene.Content.Option(
          ImmutableArray.Create<Scene.Content>(optionContent.ToArray())));


    private static ContentParser settingContent(ContentListParser parseLines) =>
      Parser.Try(Parser.Char('$'))
      .Then(NameParser.InlineWhitespace)
      .Then(Parser.OneOf(
          nameContent,
          isStartContent,
          showAlwaysContent,
          forceShowContent,
          tagsContent,
          categoryContent,
          setContent,
          requireContent,
          nextSceneContent,
          optionContent(parseLines),
          conditionalContent(parseLines)))
      .Before(NameParser.InlineWhitespace);

    private readonly static Parser<char, Scene.Content> textContent =
      Parser.AnyCharExcept("$\r\n") // not EOL or setting starting with $
      .Then(
        NameParser.RemainingLine, (lead, tail) => lead + tail)
      .Map(text => new Scene.Content.Text(text) as Scene.Content);

    private readonly static ContentParser emptyLine =
      Parser.Lookahead(Parser.EndOfLine)
      .WithResult<Scene.Content>(new Scene.Content.Text(""));


    private static ContentParser parseLine(ContentListParser parseLines) =>
      // parseLine does not handle $else or $end.
      notElseOrEnd
      .Then(
        settingContent(parseLines)
        .Or(textContent)
        .Or(emptyLine));


    private static ContentListParser parseLines(ContentListParser self) =>
      parseLine(self)
      .SeparatedAndOptionallyTerminated(Parser.EndOfLine);


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

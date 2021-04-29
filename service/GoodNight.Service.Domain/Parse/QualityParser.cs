using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Pidgin;
using GoodNight.Service.Domain.Model.Parse;

namespace GoodNight.Service.Domain.Parse
{
  // using QualityMapper = Func<Quality, Quality>;
  // using QualityParser = Parser<char, IEnumerable>;
  using ContentParser = Parser<char, Quality.Content>;
  using ContentListParser = Parser<char, IEnumerable<Quality.Content>>;


  public static class QualityParser
  {
    private readonly static ContentParser nameContent =
      Parser.Try(Parser.String("name"))
      .Then(NameParser.Colon)
      .Then(NameParser.QualityName)
      .Map<Quality.Content>(name => new Quality.Content.Name(name.Trim()));

    private readonly static ContentParser iconContent =
      Parser.Try(Parser.String("icon"))
      .Then(NameParser.Colon)
      .Then(NameParser.IconName)
      .Map<Quality.Content>(name => new Quality.Content.Name(name.Trim()));

    private readonly static ContentListParser tagsContent =
      Parser.Try(Parser.String("tag"))
      .Then(Parser.Char('s').Optional())
      .Then(NameParser.Colon)
      .Then(NameParser.TagName.Separated(Parser.Char(',')))
      .Map<IEnumerable<Quality.Content>>(tags =>
        tags.Select(t =>
          new Quality.Content.Tag(t.Trim())));

    private readonly static ContentParser categoryContent =
      Parser.Try(Parser.String("cat")
        .Then(Parser.String("egory").Optional()))
      .Then(NameParser.Colon)
      .Then(NameParser.CategoryName.Separated(Parser.Char('/')))
      .Map<Quality.Content>(cs => new Quality.Content.Category(
          ImmutableArray.CreateRange(cs)));

    private readonly static ContentParser typeContent =
      Parser.Try(Parser.String("type"))
      .Then(NameParser.Colon)
      .Then(Parser.OneOf(
        Parser.CIString("bool")
        .Then(Parser.Try(Parser.CIString("ean")).Optional())
        .WithResult<Quality.Content>(new Quality.Content.Type(
            Model.Expressions.Type.Bool)),

        Parser.CIString("int")
        .Then(Parser.Try(Parser.CIString("eger")).Optional())
        .WithResult<Quality.Content>(new Quality.Content.Type(
            Model.Expressions.Type.Int)),

        Parser.CIString("enum")
        .WithResult<Quality.Content>(new Quality.Content.Type(
            Model.Expressions.Type.Enum))));


    private readonly static ContentParser hiddenContent =
      Parser.Try(Parser.String("hidden"))
      .WithResult<Quality.Content>(new Quality.Content.Hidden());


    private readonly static ContentParser levelContent =
      Parser.Try(Parser.String("level"))
      .Then(NameParser.InlineWhitespace)
      .Then(Parser.DecimalNum)
      .Before(NameParser.Colon)
      .Then<string, Quality.Content>(NameParser.RemainingLine,
        (int num, string text) => new Quality.Content.Level(num, text));

    private readonly static ContentParser sceneContent =
      Parser.Try(Parser.String("scene"))
      .Then(NameParser.Colon)
      .Then(NameParser.SceneName)
      .Map<Quality.Content>(name => new Quality.Content.Scene(name));


    private readonly static ContentParser minContent =
      Parser.Try(Parser.String("min")
        .Then(Parser.String("imum").Optional()))
      .Before(NameParser.Colon)
      .Then(Parser.DecimalNum)
      .Map<Quality.Content>(num => new Quality.Content.Minimum(num));

    private readonly static ContentParser maxContent =
      Parser.Try(Parser.String("max")
        .Then(Parser.String("imum").Optional()))
      .Before(NameParser.Colon)
      .Then(Parser.DecimalNum)
      .Map<Quality.Content>(num => new Quality.Content.Maximum(num));


    private static ContentListParser ToList<T>(Parser<char, T> parser)
      where T : Quality.Content =>
      parser.Map<IEnumerable<Quality.Content>>(content => new[] { content });

    private readonly static ContentListParser settingContent =
      Parser.Char('$')
      .Then(NameParser.InlineWhitespace)
      .Then(Parser.OneOf(
          ToList(nameContent),
          ToList(iconContent),
          tagsContent,
          ToList(categoryContent),
          ToList(typeContent),
          ToList(hiddenContent),
          ToList(levelContent),
          ToList(sceneContent),
          ToList(minContent),
          ToList(maxContent)
          ))
      .Before(NameParser.InlineWhitespace);

    private readonly static ContentParser textContent =
      Parser.AnyCharExcept("$\r\n") // not EOL or setting starting with $
      .Then(NameParser.RemainingLine, (lead, tail) => lead + tail)
      .Map<Quality.Content>(text => new Quality.Content.Text(text));

    private readonly static ContentParser emptyLine =
      Parser.Lookahead(Parser.EndOfLine)
      .WithResult<Quality.Content>(new Quality.Content.Text(""));


    private static ContentListParser parseLine(ContentListParser parseLines) =>
      Parser.Try(settingContent)
      .Or(Parser.Try(ToList(textContent)))
      .Or(ToList(emptyLine));


    private static ContentListParser parseLines(ContentListParser self) =>
      Parser.Try(parseLine(self))
      .SeparatedAndOptionallyTerminated(Parser.EndOfLine)
      // flatten due to double wraping of settings
      .Map<IEnumerable<Quality.Content>>(c => c.SelectMany(c => c).ToArray());


    private readonly static ContentListParser parseLinesRec =
      Parser.Rec<char, IEnumerable<Quality.Content>>(parseLines);



    public static ParseResult<Quality> Parse(string content)
    {
      var res = parseLinesRec
        .Before(Parser<char>.End)
        .Parse(content);

      return res.Success
        ? new ParseResult.Success<Quality>(
          new Quality(ImmutableList.CreateRange(res.Value)))
        : ParseResult.Failure<Quality>.OfError(res.Error);
    }
  }
}

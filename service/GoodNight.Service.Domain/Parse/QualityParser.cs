using System.Linq;
using System;
using GoodNight.Service.Domain.Write;
using Pidgin;
using System.Collections.Immutable;

namespace GoodNight.Service.Domain.Parse
{
  using QualityMapper = Func<Quality, Quality>;
  using QualityFuncParser = Parser<char, Func<Quality, Quality>>;

  public class QualityParser
  {
    private readonly static Parser<char, string> remainingLine =
      Parser.AnyCharExcept("\r\n").ManyString();


    private readonly static QualityFuncParser nameContent =
      Parser.Try(Parser.String("name"))
      .Then(NameParser.Colon)
      .Then(NameParser.QualityName)
      .Map<QualityMapper>(name => 
        quality => quality with { Name = name });

    private readonly static QualityFuncParser typeContent =
      Parser.Try(Parser.String("type"))
      .Then(NameParser.Colon)
      .Then(Parser.OneOf(
        Parser.CIString("bool")
        .Then(Parser.Try(Parser.CIString("ean")).Optional())
        .WithResult<QualityMapper>(quality =>
          new Quality.Bool(quality.Name,
            quality.Raw,
            quality.Hidden,
            quality.Scene,
            quality.Description)),

        Parser.CIString("int")
        .Then(Parser.Try(Parser.CIString("eger")).Optional())
        .WithResult<QualityMapper>(quality =>
          new Quality.Int(quality.Name,
            quality.Raw,
            quality.Hidden,
            quality.Scene,
            quality.Description,
            null,
            null)),

        Parser.CIString("enum")
        .WithResult<QualityMapper>(quality =>
          new Quality.Enum(quality.Name,
            quality.Raw,
            quality.Hidden,
            quality.Scene,
            quality.Description,
            ImmutableDictionary<int, string>.Empty))));


    private readonly static QualityFuncParser hiddenContent =
      Parser.Try(Parser.String("hidden"))
      .Map<QualityMapper>(name => 
        quality => quality with { Hidden = true });


    private readonly static QualityFuncParser levelContent =
      Parser.Try(Parser.String("level"))
      .Then(NameParser.InlineWhitespace)
      .Then(Parser.DecimalNum)
      .Before(NameParser.Colon)
      .Then<string, QualityMapper>(NameParser.RemainingLine,
        (int num, string text) =>
        quality => {
          switch (quality)
          {
            case Quality.Enum e:
              return e with { Levels = e.Levels.Add(num, text.Trim()) };
            default:
              return quality;
          }
        });

    private readonly static QualityFuncParser sceneContent =
      Parser.Try(Parser.String("scene"))
      .Then(NameParser.Colon)
      .Then(NameParser.SceneName)
      .Map<QualityMapper>(name => 
        quality => quality with { Scene = name });


    private readonly static QualityFuncParser minContent =
      Parser.Try(Parser.String("min")
        .Then(Parser.String("imum").Optional()))
      .Before(NameParser.Colon)
      .Then(Parser.DecimalNum)
      .Map<QualityMapper>(num => quality => {
          switch (quality)
          {
            case Quality.Int e:
              return e with { Minimum = num };
            default:
              return quality;
          }
        });

    private readonly static QualityFuncParser maxContent =
      Parser.Try(Parser.String("max")
        .Then(Parser.String("imum").Optional()))
      .Before(NameParser.Colon)
      .Then(Parser.DecimalNum)
      .Map<QualityMapper>(num => quality => {
          switch (quality)
          {
            case Quality.Int e:
              return e with { Maximum = num };
            default:
              return quality;
          }
        });


    private readonly static QualityFuncParser settingContent =
      Parser.Char('$')
      .Then(NameParser.InlineWhitespace)
      .Then(Parser.OneOf(
          nameContent,
          typeContent,
          hiddenContent,
          levelContent,
          sceneContent,
          minContent,
          maxContent
          ))
      .Before(NameParser.InlineWhitespace)
      .Select<QualityMapper>(c => c);

    private readonly static QualityFuncParser textContent =
      Parser.AnyCharExcept("$\r\n")
      .Then(remainingLine, (lead, tail) => lead + tail)
      .Map<QualityMapper>(text =>
        quality => quality with {
          Description = quality.Description + "\n" + text });

    private readonly static QualityFuncParser emptyLine =
      Parser.Lookahead(Parser.EndOfLine)
      .WithResult<QualityMapper>(
        quality => quality with {
          Description = quality.Description + "\n" });

    private readonly static QualityFuncParser parseLine =
      Parser.Try(settingContent)
      .Or(Parser.Try(textContent))
      .Or(emptyLine);

    private readonly static Parser<char, Quality> parseLines =
      Parser.Try(parseLine
        // .Trace(f => $"--debut \"{f}\"  parsed a line!")
      )
      .SeparatedAndOptionallyTerminated(Parser.EndOfLine)
      .Map(builders => builders.Aggregate(
          new Quality.Bool("", "", false, null, "") as Quality,
          (a, f) => f(a)));



    public ParseResult<Quality> Parse(string content)
    {
      var res = parseLines
        .Before(Parser<char>.End)
        .Parse(content);

      return new ParseResult<Quality>(res.Success,
        (res.Success
          ? (res.Value.Description.StartsWith("\n") 
            ? res.Value with {
              Description = res.Value.Description.Substring(1) }
            : res.Value)
          : null),
        !res.Success && res.Error is not null ? res.Error.Message : null,
        (!res.Success && res.Error is not null
          ? new Tuple<int,int>(res.Error.ErrorPos.Line, res.Error.ErrorPos.Col)
          : null),
        (!res.Success && res.Error is not null && res.Error.Unexpected.HasValue
          ? res.Error.Unexpected.Value.ToString()
          : null),
        (!res.Success && res.Error is not null
          ? String.Join(", ", res.Error.Expected.Select(e => e.ToString()))
          : null));
    }
  }
}

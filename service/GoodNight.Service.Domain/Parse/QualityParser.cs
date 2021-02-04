using System.Linq;
using System;
using GoodNight.Service.Domain.Write;
using Pidgin;

namespace GoodNight.Service.Domain.Parse
{
  using QualityFuncParser = Parser<char, Func<Quality, Quality>>;

  public class QualityParser
  {
    private readonly static QualityFuncParser settingContent =
      ;

    private readonly static QualityFuncParser textContent =
      ;

    private readonly static QualityFuncParser emptyLine =
      ;

    private readonly static QualityFuncParser parseLine =
      Parser.Try(settingContent)
      .Or(Parser.Try(textContent))
      .Or(emptyLine);

    private readonly static Parser<char, Quality> parseLines =
      Parser.Try(parseLine)
      .SeparatedAndOptionallyTerminated(Parser.EndOfLine)
      .Map(builders => builders.Aggregate(
          new Quality.Bool("", "", false, null, ""),
          (a, f) => f(a)));



    public ParseResult<Quality> Parse(string content)
    {
      var res = parseLines
        .Before(Parser<char>.End)
        .Parse(content);

      return new ParseResult<Quality>(res.Success,
        res.Success ? res.Value : null,
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

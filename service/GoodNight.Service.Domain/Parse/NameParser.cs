using Pidgin;

namespace GoodNight.Service.Domain.Parse
{
  internal static class NameParser
  {
    private readonly static Parser<char, char> qualityStarters =
      Parser.Letter;

    internal readonly static Parser<char, char> QualityLetters =
      Parser.LetterOrDigit.Or(Parser.OneOf("_"));


    private readonly static Parser<char, Unit> noKeyword =
      Parser.Try(Parser.Not(Parser.Try(Parser.OneOf(
              Parser.String("and"),
              Parser.String("or"),
              Parser.String("true"),
              Parser.String("false"),
              Parser.String("not"))
            .Before(Parser.Not(QualityLetters)))));

    internal readonly static Parser<char, string> QualityName =
        //  a quality in quotes, this can be anything.
      Parser.Try(Parser.AnyCharExcept("\r\n\"")
        .ManyString()
        .Between(Parser.Char('"')))

        .Or(noKeyword
          // a quality without spaces.
          .Then(qualityStarters)
          .Then(QualityLetters.ManyString(), (char f, string r) => f + r)

          // except there may be spaces, if there are no keywords.
          .Then(Parser.Try(
              Parser.OneOf(" \t").AtLeastOnceString()
              .Then(noKeyword
                .Then(QualityLetters.ManyString()),
                (string f, string r) => f + r)
            ).ManyString(),
            (string f, string r) => (f + r).Trim()));


    internal readonly static Parser<char, string> InlineWhitespace =
      Parser.OneOf(" \t").ManyString();

    internal readonly static Parser<char, Unit> Colon =
      InlineWhitespace
      .Then(Parser.Char(':'))
      .Then(NameParser.InlineWhitespace)
      .WithResult(Unit.Value);

    internal readonly static Parser<char, string> RemainingLine =
      Parser.AnyCharExcept("\r\n").ManyString();


    private readonly static Parser<char, char> nameCommonLetters =
      Parser.LetterOrDigit
      .Or(Parser.OneOf("_- !\"%^&*()=+[]{}`;:'@#~|<>.?\t"));


    internal readonly static Parser<char, string> IconName =
      Parser.LetterOrDigit.Or(Parser.OneOf("_- ")).ManyString()
      .Map(name => name.Trim());

    internal readonly static Parser<char, string> SceneName =
      nameCommonLetters.Or(Parser.OneOf(",\\/")).ManyString()
      .Map(name => name.Trim());

    internal readonly static Parser<char, string> TagName =
      nameCommonLetters.Or(Parser.OneOf("\\/")).ManyString()
      .Map(name => name.Trim());

    internal readonly static Parser<char, string> CategoryName =
      nameCommonLetters.Or(Parser.OneOf(",")).ManyString()
      .Map(name => name.Trim());

  }
}

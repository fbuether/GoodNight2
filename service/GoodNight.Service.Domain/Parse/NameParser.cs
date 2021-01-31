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

    // Names of qualities are letters, digits, underscores and spaces if quoted.
    internal readonly static Parser<char, string> QualityName =
        //  a quality in quotes, this can be anything.
        qualityStarters
        .Then(QualityLetters.Or(Parser.Char(' ')).ManyString(), (f,r) => f + r)
        .Between(Parser.Char('"'))

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
            (string f, string r) => f + r));


    internal readonly static Parser<char, string> InlineWhitespace =
      Parser.OneOf(" \t").ManyString();
  }
}

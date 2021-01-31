using Pidgin;
using GoodNight.Service.Domain.Write;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections.Immutable;
using GoodNight.Service.Domain.Write.Expressions;

namespace GoodNight.Service.Domain.Parse
{
  internal static class NameParser
  {
    private readonly static Parser<char, char> qualityStarters =
      Parser.Letter;

    internal readonly static Parser<char, char> QualityLetters =
      Parser.LetterOrDigit.Or(Parser.OneOf("_"));


    private readonly static Parser<char, Unit> noKeyword =
      Parser.Try(
        Parser.Not(
          Parser.Try(
            Parser.OneOf(
              Parser.String("and"),
              Parser.String("or"),
              Parser.String("true"),
              Parser.String("false"),
              Parser.String("not")
            )
          // .Trace(f => $"-- debut \"{f}\" found a word starting with keyword in quality name")
          .Before(Parser.Not(QualityLetters))
          )
          // .Trace(f => $"-- debut \"{f}\" found a keyword in quality name")
        )
        // .Then(Parser.Lookahead(Parser<char>.Any.ManyString()).Trace(f => $"-- debut \"{f}\" remainder inner-post kw"))
        // .Trace(f => $"-- debut \"{f}\" found not a keyword in quality name")
      )
      //     .Then(Parser.Lookahead(Parser<char>.Any.ManyString()).Trace(f => $"-- debut \"{f}\" remainder inner-out-post kw"))
      // .Trace(f => $"-- debut \"{f}\" succeeded trying to not find a keyword in quality name")
      .WithResult(Unit.Value);

    // Names of qualities are letters, digits, underscores and spaces if quoted.
    internal readonly static Parser<char, string> QualityName =
      // Parser<char>.FromResult(Unit.Value).Trace(f => $"-- debut \"{f}\" starting qualityname")
      // .Then(

        //  a quality in quotes, this can be anything.
        qualityStarters
        .Then(QualityLetters.Or(Parser.Char(' ')).ManyString(), (f,r) => f + r)
        .Between(Parser.Char('"'))

        // a quality without spaces.
        .Or(
          noKeyword// .Trace(f => $"-- debut \"{f}\" no keyword comin up.")
          // .Then(Parser.Lookahead(Parser<char>.Any.ManyString()).Trace(f => $"-- debut \"{f}\" remainder for quality"))

          .Then(qualityStarters)
          .Then(QualityLetters.ManyString(), (char f, string r) => f + r)

          // .Trace(f => $"-- debut \"{f}\" first quality word")

          // try collect more words, unless they are keywords
          .Then(
            Parser.Try(
              Parser.OneOf(" \t").AtLeastOnceString()
          //     .Trace(f => $"-- debut \"{f}\" quality followup whitespace")
          // .Then(Parser.Lookahead(Parser<char>.Any.ManyString()).Trace(f => $"-- debut \"{f}\" remainder pre keywords"))
              .Then(
      // Parser<char>.FromResult(Unit.Value)// .Trace(f => $"-- debut \"{f}\" pre keyword")
          // .Then(Parser.Lookahead(Parser<char>.Any.ManyString()).Trace(f => $"-- debut \"{f}\" remainder pre keywords"))
      // .Then(
                // A quality name, but not a keyword.
                noKeyword

// )
          // .Then(Parser.Lookahead(Parser<char>.Any.ManyString()).Trace(f => $"-- debut \"{f}\" remainder post keywords"))
              // .Trace(f => $"-- debut \"{f}\" quality pre post whitespace text")
                .Then(QualityLetters.ManyString()
              // .Trace(f => $"-- debut \"{f}\" quality post whitespace text")

                ),
                (string f, string r) => f + r)
              // maybe several space-seperated words appended?
              // .ManyString()
              // .Trace(f => $"-- debut \"{f}\" quality valid further word")
            ).ManyString(),
            (f, r) => f + r)
            // (string f, Maybe<string> r) => f + r.GetValueOrDefault(""))


          // .Trace(f => $"-- debut \"{f}\" quality with spaces finished")
        // )

);


    internal readonly static Parser<char, string> InlineWhitespace =
      Parser.OneOf(" \t").ManyString();

  }
}

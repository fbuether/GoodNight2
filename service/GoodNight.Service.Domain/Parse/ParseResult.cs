using System;
using System.Linq;
using GoodNight.Service.Domain.Util;

namespace GoodNight.Service.Domain.Parse
{
  public abstract record ParseResult<T>()
  {
    public abstract Result<T, string> ToResult();
  };

  public static class ParseResult
  {
    public sealed record Success<T>(
      /// <summary>
      /// Contains the parser result.
      /// </summary>
      T Result)
      : ParseResult<T>
    {
      public override Result<T, string> ToResult()
      {
        return new Result.Success<T, string>(Result);
      }
    }

    public sealed record Failure<T>(
      /// <summary>
      /// Contains the human-readable error message.
      /// </summary>
      string ErrorMessage,

      /// <summary>
      /// The position of the error as a tuple of <line,col>.
      /// </summary>
      Tuple<int,int> ErrorPosition,

      /// <summary>
      /// The unexpected token, if this was caused by an unexpected token.
      /// </summary>
      string? UnexpectedToken,

      /// <summary>
      /// The token expected, if this was caused by an unexpected token.
      /// </summary>
      string? ExpectedToken)
      : ParseResult<T>
    {
      public override Result<T, string> ToResult()
      {
        return new Result.Failure<T, string>(
          $"Parse Error: {ErrorMessage}\nPosition: {ErrorPosition}\n"
          + $"Found Token: \"{UnexpectedToken}\"\n"
          + $"Expected Token: \"{ExpectedToken}\"");
      }

      public static Failure<T> OfError(Pidgin.ParseError<Char>? error)
      {
        if (error is null)
        {
          return new ParseResult.Failure<T>(
            "Parsing was not successful, but no error was produced.",
            Tuple.Create(0, 0), null, null);
        }

        string expr = error.Expected.Count() == 1
          && error.Expected.First().Label is not null
          ? error.Expected.First().Label!
          : "\"" + string.Join("\", \"", error.Expected) + "\"";

        var expectMessage =
          "At line " + error.ErrorPos.Line + ", pos " + error.ErrorPos.Col +
          ", expected " +
          (!error.Expected.Any()
            ? "nothing"
            : expr) +
          (error.Unexpected.HasValue
            ? ", but found \"" + (error.Unexpected.Value == '\n'
              ? "\\n"
              : error.Unexpected.Value) + "\"."
            : ".");

        return new ParseResult.Failure<T>(
          error.Message ?? expectMessage,
          Tuple.Create(error.ErrorPos.Line, error.ErrorPos.Col),
          error.Unexpected.HasValue
            ? error.Unexpected.Value.ToString()
            : "",
          String.Join(", ", error.Expected.Select(e => e.ToString())));
      }
    }
  }
}

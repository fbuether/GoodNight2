using System;

namespace GoodNight.Service.Domain.Parse
{
  public record ParseResult<T>(
    /// <summary>
    /// Did the parsing succeed?
    /// </summary>
    bool IsSuccessful,

    /// <summary>
    /// Contains the parser result if parsing was successful, null otherwise.
    /// </summary>
    T? Result,

    /// <summary>
    /// Contains the human-readable error message, if parsing was un-successful,
    /// null otherwise.
    /// </summary>
    string? ErrorMessage,

    /// <summary>
    /// The position of the error, if parsing was un-successful,
    /// null otherwise. This is a tuple of <line,col>.
    /// </summary>
    Tuple<int,int>? ErrorPosition,

    /// <summary>
    /// The unexpected token, if parsing was un-successful and this was caused
    /// by an unexpected token.
    /// </summary>
    string? UnexpectedToken,

    /// <summary>
    /// The token expected, if parsing was un-successful and this was caused
    /// by an unexpected token.
    /// </summary>
    string? ExpectedToken)
  {
  }
}

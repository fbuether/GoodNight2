using Pidgin;
using GoodNight.Service.Domain.Write;
using System.Collections.Generic;

namespace GoodNight.Service.Domain.Parse
{
  public class SceneParser
  {
    private readonly static Parser<char, Unit> inlineWhitespace =
      Parser.OneOf(" \t").SkipMany();

    private readonly static Parser<char, Unit> endOfLine =
      Parser.OneOf("\r\n").SkipAtLeastOnce();

    private readonly static Parser<char, string> remainingLine =
      Parser.AnyCharExcept("\r\n").ManyString();

    private readonly static Parser<char, Content> nameContent =
      Parser.String("name")
      .Then(inlineWhitespace)
      .Then(Parser.Char(':'))
      .Then(remainingLine)
      .Map<Content>(name => new Content.Name(name.Trim()));

    private readonly static Parser<char, Content> settingContent =
      Parser.Char('$')
      .Then(inlineWhitespace)
      .Then(
        nameContent
      );

    private readonly static Parser<char, Content> textContent =
      Parser.Map(
        (first, remainder) => first + remainder,
        Parser.AnyCharExcept("$"),
        remainingLine)
      .Map<Content>(text => new Content.Text(text));


    private readonly static Parser<char, IEnumerable<Content>> parseLines =
      settingContent
      .Or(textContent)
      .Separated(endOfLine);

    public Scene? Parse(string content)
    {
      var result = parseLines.Parse(content);
      if (result.Success)
      {
        System.Console.WriteLine("successful parse.");
        foreach (var val in result.Value)
        {
          System.Console.WriteLine($"value: {val}");
        }
      }
      else
      {
        System.Console.WriteLine($"parse failed :( {result.Error}");
      }

      return null;
    }
  }
}

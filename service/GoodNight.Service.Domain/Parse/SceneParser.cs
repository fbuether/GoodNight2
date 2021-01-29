using Pidgin;
using GoodNight.Service.Domain.Write;
using System.Collections.Generic;

namespace GoodNight.Service.Domain.Parse
{
  public class SceneParser
  {
    private readonly static Parser<char, Unit> inlineWhitespace =
      Parser.Char(' ').Or(Parser.Char('\t')).SkipMany();

    private readonly static Parser<char, Unit> endOfLine =
      Parser.Char('\n').Or(Parser.Char('\r')).SkipAtLeastOnce();

    private readonly static Parser<char, string> remainingLine =
      Parser.AnyCharExcept("\r\n").ManyString();

    private readonly static Parser<char, Content> parseName =
      Parser.Char('$')
      .Then(inlineWhitespace)
      .Then(Parser.String("name"))
      .Then(inlineWhitespace)
      .Then(Parser.Char(':'))
      .Then(remainingLine)
      .Map<Content>(name => new Content.Name(name));

    private readonly static Parser<char, Content> parseSetting =
      Parser.Char('$').Then(
        parseName
      );



    private readonly static Parser<char, IEnumerable<Content>> parseLines =
      parseSetting
      .Or(remainingLine.Map<Content>(text => new Content.Text(text)))
      .Many();

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

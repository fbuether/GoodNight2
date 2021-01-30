using GoodNight.Service.Domain.Write.Expressions;
using Pidgin;

namespace GoodNight.Service.Domain.Parse
{
  internal class ExpressionParser
  {
    internal readonly static Parser<char, Expression> expression =
      Parser.String("true")
      .Map<Expression>(_ => new Expression.Bool(true));
  }
}

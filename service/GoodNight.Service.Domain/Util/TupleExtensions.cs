using System;

namespace GoodNight.Service.Domain.Util
{
  public static class TupleExtensions
  {
    public static (T3,T2) MapFirst<T1,T2,T3>(this (T1,T2) tuple, Func<T1,T3> f)
    {
      var (t1,t2) = tuple;
      var t3 = f(t1);
      return (t3, t2);
    }
  }
}

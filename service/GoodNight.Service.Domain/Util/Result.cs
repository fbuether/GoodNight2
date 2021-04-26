using System;
using System.Diagnostics.CodeAnalysis;

namespace GoodNight.Service.Domain.Util
{
  public abstract record Result<TResult, TError>
  {
    public abstract T Map<T>(Func<TResult, T> onSuccess,
      Func<TError, T> onError);

    public abstract Result<TResult, TError> Filter(
      Func<TResult, bool> predicate, TError error);

    public abstract Result<T, TError> Map<T>(Func<TResult, T> onSuccess);

    public abstract TResult GetOrError(Func<TError, TResult> transformer);

    public abstract Result<T, TError> Bind<T>(
      Func<TResult, Result<T, TError>> transformer);

    public abstract Result<TResult, TError> Do(Action<TResult> action);
  }

  public static class Result
  {
    public sealed record Success<TResult, TError>(TResult Result)
      : Result<TResult, TError>
    {
      public override Result<T, TError> Bind<T>(
        Func<TResult, Result<T, TError>> transformer)
      {
        return transformer(Result);
      }

      public override Result<TResult, TError> Do(Action<TResult> action)
      {
        action(Result);
        return this;
      }

      public override Result<TResult, TError> Filter(
        Func<TResult, bool> predicate, TError error)
      {
        return predicate(Result)
          ? this
          : new Result.Failure<TResult, TError>(error);
      }

      public override TResult GetOrError(Func<TError, TResult> transformer)
      {
        return Result;
      }

      public override T Map<T>(Func<TResult, T> onSuccess,
        Func<TError, T> onError)
      {
        return onSuccess(this.Result);
      }

      public override Result<T, TError> Map<T>(Func<TResult, T> onSuccess)
      {
        return new Result.Success<T, TError>(onSuccess(this.Result));
      }
    }

    public sealed record Failure<TResult, TError>(TError Error)
      : Result<TResult, TError>
    {
      public override Result<T, TError> Bind<T>(
        Func<TResult, Result<T, TError>> transformer)
      {
        return new Result.Failure<T, TError>(Error);
      }

      public override Result<TResult, TError> Do(Action<TResult> action)
      {
        return this;
      }

      public override Result<TResult, TError> Filter(
        Func<TResult, bool> predicate, TError error)
      {
        return this;
      }

      public override TResult GetOrError(Func<TError, TResult> transformer)
      {
        return transformer(Error);
      }

      public override T Map<T>(Func<TResult, T> onSuccess,
        Func<TError, T> onError)
      {
        return onError(this.Error);
      }

      public override Result<T, TError> Map<T>(Func<TResult, T> onSuccess)
      {
        return new Result.Failure<T, TError>(Error);
      }
    }

    public static T Return<T>(Result<T,T> result)
    {
      switch (result)
      {
        case Result.Success<T,T> s:
          return s.Result;

        case Result.Failure<T,T> f:
          return f.Error;

        default:
          throw new InvalidOperationException();
      }
    }

    public static Result<T, TError> FailOnNull<T, TError>(T? value,
      TError error)
    {
      return value is not null
        ? new Result.Success<T, TError>(value)
        : new Result.Failure<T, TError>(error);
    }
  }
}

using System;
using System.Collections.Immutable;
using GoodNight.Service.Domain.Model.Expressions;
using GoodNight.Service.Storage.Interface;

namespace GoodNight.Service.Domain.Model.Write
{
  public record Scene(
    string Name,
    string Raw,
    bool IsStart,
    bool ShowAlways,
    bool ForceShow,
    IImmutableList<string> Tags,
    IImmutableList<string> Category,

    IImmutableList<(string, Expression)> Sets,

    string? Return,
    string? Continue,

    IImmutableList<Content> Content)
    : IStorable<string>
  {

    public string Urlname
    {
      get
      {
        return NameConverter.OfString(Name);
      }
    }

    public string GetKey()
    {
      return Urlname;
    }

    public static Scene CreateDefault()
    {
      return new Scene("", "", false, false, false,
        ImmutableList<string>.Empty,
        ImmutableList<string>.Empty,
        ImmutableList<(string, Expression)>.Empty,
        null,
        null,
        ImmutableList<Content>.Empty);
    }

    public Scene AddContent(Content newContent)
    {
      return this with { Content = Content.Add(newContent) };
    }



    // public ReadScene Play(IImmutableDictionary<string, Value> qualities)
    // {
    //   throw new NotImplementedException();
    //   // return new ReadScene();
    // }
  }
}

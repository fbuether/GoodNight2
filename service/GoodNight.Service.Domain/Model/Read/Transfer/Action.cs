using System.Collections.Immutable;
using GoodNight.Service.Domain.Model.Expressions;

namespace GoodNight.Service.Domain.Model.Read.Transfer
{
  public record Requirement(
    Expression<QualityHeader> Expression,
    bool Passed);

  public record Option(
    string Urlname,
    string Text,
    string? Icon,
    bool IsAvailable,
    IImmutableList<Requirement> Requirements,
    IImmutableList<Property> Effects,
    string Scene);

  public record Action(
    string Text,
    IImmutableList<Property> Effects,
    IImmutableList<Option> Options,
    string? Return,
    string? Continue);
}

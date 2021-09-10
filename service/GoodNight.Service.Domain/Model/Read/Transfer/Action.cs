using System.Collections.Immutable;

namespace GoodNight.Service.Domain.Model.Read.Transfer
{
  public record Requirement(
    string Display,
    string? Icon,
    bool Passed);

  public record Test(
    string Display,
    string? Icon,
    int Chance);

  public record Option(
    string Urlname,
    string Text,
    string? Icon,
    bool IsAvailable,
    IImmutableList<Test> Tests,
    IImmutableList<Requirement> Requirements);

  public record Action(
    string Text,
    IImmutableList<Property> Effects,
    IImmutableList<Option> Options,
    bool HasReturn,
    bool HasContinue);
}

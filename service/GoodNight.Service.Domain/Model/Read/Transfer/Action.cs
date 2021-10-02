using System.Collections.Immutable;

namespace GoodNight.Service.Domain.Model.Read.Transfer
{
  // Requirements stem from an expression of type bool
  public record Requirement(
    string Display,
    string? Icon,
    bool? Passed);

  // Tests stem from requirements with expressions of type chance
  public record Test(
    string Display,
    string? Icon,
    int Chance);

  public record Option(
    string Urlname,
    string Text,
    bool IsAvailable,
    IImmutableList<Requirement> Requirements,
    IImmutableList<Test> Tests);

  public record Action(
    string Text,
    IImmutableList<Property> Effects,
    IImmutableList<Option> Options,
    bool HasReturn,
    bool HasContinue);
}

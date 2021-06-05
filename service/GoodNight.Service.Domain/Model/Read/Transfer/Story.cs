namespace GoodNight.Service.Domain.Model.Read.Transfer
{
  /// <summary>
  /// A short summary of a story. This is mostly used for serialisation and
  /// transfer to the client, as the client never sees the whole story.
  /// </summary>
  public record Story(
    string Name,
    string Urlname,
    string? Icon,
    string Description);
}

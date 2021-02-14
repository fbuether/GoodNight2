using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using GoodNight.Service.Domain.Read;
using GoodNight.Service.Storage;

using System.Collections.Immutable;
using GoodNight.Service.Domain.Model.Expressions;
using Action = GoodNight.Service.Domain.Read.Action;

namespace GoodNight.Service.Api.Play
{
  [ApiController]
  [Route("api/v1/read")]
  public class ReadStoryController : ControllerBase
  {
    private IStore store;

    public ReadStoryController(IStore store)
    {
      this.store = store;
    }


    [HttpGet("continue")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Player> GetPlayerState()
    {
      var muenzen = new Quality.Int("Münzen",
        "Alles, was glitzert, ist Gold.", null);
      var finasHammer = new Quality.Bool("Finas Hammer",
        "Der mächtige Hammer der Schmiedin.", null);

      var player = new Player("logged_in_user", "Mrs. Hollywinkle",
        ImmutableArray.Create<Property>(
          new Property(muenzen, new Value.Int(4)),
          new Property(finasHammer, new Value.Bool(true))
        ));

      var story = new Story("Hels Schlucht", "hels-schlucht",
        "Hels Schlucht ist die letzte Zuflucht der Menschen", true);
      var history = ImmutableList.Create<Action>(
        new Action("start",
          "Die Zeit der Menschen ist vorbei, sagen…",
          new Choice.Continue("start-fortsetzung")));

      var current =
        new Scene(
          "start-fortsetzung",
          "# Weiter auf der Flucht\n\nAuch du bist geflohen, dem Ruf…",
          ImmutableList.Create<Property>(),
          ImmutableList.Create<Option>(new[] {
              new Option("herkunft-kueste", true, "an der Küste zwischen dem…",
                ImmutableList<Requirement>.Empty),
              new Option("herkunft-steppe", true, "die weiten, baumlosen…",
                ImmutableList<Requirement>.Empty),
              new Option("herkunft-berge", true, "hoch an den Bergspitzen…",
                ImmutableList<Requirement>.Empty)
            }),
          null,
          null);

      return Ok(new Adventure(player, story, history, current));
    }
  }
}

using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using GoodNight.Service.Storage.Interface;
using GoodNight.Service.Domain.Model;
using System;

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
    public ActionResult<Adventure> GetAdventure()
    {
      var muenzen = new Quality.Int("Münzen", "two-coins",
        "Alles, was glitzert, ist Gold.", null);
      var finasHammer = new Quality.Bool("Finas Hammer", "shamrock",
        "Der mächtige Hammer der Schmiedin.", null);

      var player = new Player("logged_in_user", "Mrs. Hollywinkle",
        ImmutableList.Create<Property>(
          new Property(muenzen, new Value.Int(4)),
          new Property(finasHammer, new Value.Bool(true))
        ));

      var story = new Story("Hels Schlucht", "hels-schlucht",
        "Hels Schlucht ist die letzte Zuflucht der Menschen", true);
      var history = ImmutableList.Create<Action>(
        new Action("start",
          "Die Zeit der Menschen ist vorbei, sagen…",
          ImmutableList.Create<Property>(
            new Property(muenzen, new Value.Int(2))),
          new Choice.Continue("start-fortsetzung")));

      var current =
        new Scene(
          "start-fortsetzung",
          "### Weiter auf der Flucht\n\nAuch du bist geflohen, dem Ruf…",
          ImmutableList.Create<Property>(
            new Property(finasHammer, new Value.Bool(false))
          ),
          ImmutableList.Create<Option>(new[] {
              new Option("herkunft-kueste", true, "an der Küste zwischen dem…",
                ImmutableList<Requirement>.Empty),
              new Option("herkunft-steppe", true, "die weiten, baumlosen…",
                ImmutableList<Requirement>.Empty),
              new Option("herkunft-berge", false, "hoch an den Bergspitzen…",
                ImmutableList<Requirement>.Empty)
            }),
          null,
          null);

      return Ok(new Adventure(player, story, history, current));
    }

    [HttpPost("do")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Consequence> DoOption()
    {
      var beutel = new Quality.Bool("Beutel", "swap-bag", "Zur Aufbewahrung.",
        null);
      var finasHammer = new Quality.Bool("Finas Hammer", "shamrock",
        "Der mächtige Hammer der Schmiedin.", null);

      return new Consequence(
        new Action(
          "start-fortsetzung",
          "### Weiter auf der Flucht\n\nAuch du bist geflohen, dem Ruf…",
          ImmutableList.Create<Property>(
            new Property(finasHammer, new Value.Bool(false))
          ),
          new Choice.Option("wherever", "Du tust, was du tun musst.",
            ImmutableList<Property>.Empty)),
        new Scene("start2",
          "Und so geht es weiter...",
          ImmutableList.Create<Property>(
            new Property(beutel, new Value.Bool(true))),
          ImmutableList.Create<Option>(new[] {
              new Option("naechster-schritt", true, "Und dann kommt…",
                ImmutableList<Requirement>.Empty),
              new Option("oder-anderer", true, "Hier ist erstmal Pause.",
                ImmutableList.Create<Requirement>(new[] {
                    new Requirement(new Expression.BinaryApplication(
                        new Expression.BinaryOperator.Equal(),
                        new Expression.Quality("muenzen"),
                        new Expression.Number(4)),
                      true)
                  }))
            }), "return-scene", null));
    }
  }
}

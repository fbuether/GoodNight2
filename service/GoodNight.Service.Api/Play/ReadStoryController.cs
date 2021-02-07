using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using GoodNight.Service.Domain.Read;
using GoodNight.Service.Storage;
using System.Collections.Generic;
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
      var user = "current_logged_in_user";
      var player = "Mrs. Hollywinkle";
      var story = "Hels Schlucht";
      var history = ImmutableList.Create<Action>(
        new Action("start",
          "Die Zeit der Menschen ist vorbei, sagen…",
          new Choice.Continue("start-fortsetzung")));

      var current =
        new Scene(
          "start-fortsetzung",
          "# Weiter auf der Flucht\n\nAuch du bist geflohen, dem Ruf…",
          ImmutableList.Create<(Quality, Value)>(),
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

      var state = ImmutableDictionary<string, Value>.Empty
        .Add("Münzen", new Value.Int(4))
        .Add("Finas Hammer", new Value.Bool(true));

      return Ok(new Player(user, player, story, history, current, state));
    }
  }
}

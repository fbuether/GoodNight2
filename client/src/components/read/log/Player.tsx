import {Player as State} from "../../../state/model/read/Player";

import Property from "./Property";


export default function Player(player: State) {
  return (
    <div>
      <h3>{player.name}</h3>
      <ul id="state" class="list-group">
        {player.state.map(Property)}
      </ul>
    </div>
  );
}

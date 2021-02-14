import Property from "../../components/play/Property";

import type {Player} from "../../model/read/Player";


export default function State(player: Player) {
  return (
    <div>
      <h3>{player.name}</h3>
      <ul id="state" class="list-group">
        {Array.from(player.state, (property) => 
          <Property {...property} />)}
      </ul>
    </div>
  );
}

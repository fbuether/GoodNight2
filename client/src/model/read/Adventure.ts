import {deserialiseArray} from "../Deserialise";

import {Player, asPlayer} from "./Player";
import {Story, asStory} from "./Story";
import {Log} from "./Log";
import {Action, asAction} from "./Action";


export interface Adventure {
  key: string;

  player: Player;
  user: string;
  story: Story;
  history: Array<Log>;
  current: Action;
}


// export function asAdventure(obj: any): Adventure {
//   return {
//     player: asPlayer(obj["player"]),
//     story: asStory(obj["story"]),
//     history: deserialiseArray(obj, "history", asAction),
//     current: asScene(obj["current"])
//   };
// }

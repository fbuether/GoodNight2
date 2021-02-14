import {deserialiseArray} from "../Deserialise";

import {Player, asPlayer} from "./Player";
import {Story, asStory} from "./Story";
import {Action, asAction} from "./Action";
import {Scene, asScene} from "./Scene";


export interface Adventure {
  readonly player: Player;
  readonly story: Story;
  readonly history: Array<Action>;
  readonly current: Scene;
}


export function asAdventure(obj: any): Adventure {
  return {
    player: asPlayer(obj["player"]),
    story: asStory(obj["story"]),
    history: deserialiseArray(obj, "history", asAction),
    current: asScene(obj["current"])
  };
}

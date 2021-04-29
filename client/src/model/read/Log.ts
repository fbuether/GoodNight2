// import {deserialiseArray} from "../Deserialise";

import type {Scene} from "./Scene";
import type {Property} from "./Property";
// import {Player, asPlayer} from "./Player";
// import {Story, asStory} from "./Story";
// import {Log, asLog} from "./Log";
// import {Action, asAction} from "./Action";

export interface ChoiceAction {
  kind: "action";
  urlname: string;
  text: string;
  icon: string | null;
  effects: Array<Property>;
}

export interface ChoiceReturn {
  kind: "return";
}

export interface ChoiceContinue {
  kind: "continue";
}

export type Choice =
    | ChoiceAction
    | ChoiceReturn
    | ChoiceContinue;


export interface Log {
  key: string;

  number: number;
  scene: string;
  text: string;
  effects: Array<Property>;
  chosen: Choice;
}


// export function asAdventure(obj: any): Adventure {
//   return {
//     player: asPlayer(obj["player"]),
//     story: asStory(obj["story"]),
//     history: deserialiseArray(obj, "history", asAction),
//     current: asScene(obj["current"])
//   };
// }

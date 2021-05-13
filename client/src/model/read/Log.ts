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
  number: number;
  scene: string; // urlname
  text: string;
  effects: Array<Property>;
  chosen: Choice;
}

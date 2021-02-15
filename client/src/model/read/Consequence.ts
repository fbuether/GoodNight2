
import {Action, asAction} from "./Action";
import {Scene, asScene} from "./Scene";


export interface Consequence {
  readonly action: Action;
  readonly scene: Scene;
}


export function asConsequence(obj: any): Consequence {
  return {
    action: asAction(obj["action"]),
    scene: asScene(obj["scene"])
  };
}

import {requireString, deserialiseArray} from "../Deserialise";

import {Property, asProperty} from "./Property";
import {Quality, asQuality} from "./Quality";
import {Value, asValue} from "./Value";


interface ChoiceOption {
  readonly kind: "option";
  readonly scene: string;
  readonly text: string;
  readonly effects: Array<Property>;
}

interface ChoiceReturn {
  readonly kind: "return";
  readonly scene: string;
}

interface ChoiceContinue {
  readonly kind: "continue";
  readonly scene: string;
}

export type Choice = ChoiceOption | ChoiceReturn | ChoiceContinue;


export interface Action {
  readonly urlname: string;
  readonly text: string;
  readonly chosen: Choice;
}



function asChoice(obj: any): Choice {
  let kind = requireString(obj["kind"]);
  switch (kind) {
    case "option":
      return {
        kind: kind,
        scene: requireString(obj["scene"]),
        text: requireString(obj["text"]),
        effects: deserialiseArray(obj, "effects", asProperty)
      };
    case "return":
    case "continue":
      return {
        kind: kind,
        scene: requireString(obj["scene"])
      };
    default:
      console.error("Invalid choice kind", kind);
      throw new Error("Invalid choice kind");
  }
}


export function asAction(obj: any): Action {
  return {
    urlname: requireString(obj["urlname"]),
    text: requireString(obj["text"]),
    chosen: asChoice(obj["chosen"])
  };
}

import type {Property} from "./Property";


interface ChoiceOption {
  kind: "option";
  scene: string;
  text: string;
  effects: Array<Property>;
}

interface ChoiceReturn {
  kind: "return";
  scene: string;
}

interface ChoiceContinue {
  kind: "continue";
  scene: string;
}

export type Choice = ChoiceOption | ChoiceReturn | ChoiceContinue;


export interface Action {
  urlname: string;
  text: string;
  effects: Array<Property>;
  chosen: Choice;
}

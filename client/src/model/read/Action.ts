import {Quality} from "./Quality";
import {Value} from "./Value";


interface ChoiceOption {
  kind: "option";
  scene: string;
  text: string;
  effects: Array<[Quality, Value]>;
}

interface ChoiceReturn {
  kind: "return";
}

interface ChoiceContinue {
  kind: "continue";
}

type Choice = ChoiceOption | ChoiceReturn | ChoiceContinue;


export interface Action {
  urlname: string;
  text: string;
  chosen: Choice;
}

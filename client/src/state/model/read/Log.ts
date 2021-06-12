import type {Property} from "./Property";


interface ChoiceAction {
  kind: "action";
  text: string;
  icon: string | null;
  effects: Array<Property>;
}

interface ChoiceReturn {
  kind: "return";
}

interface ChoiceContinue {
  kind: "continue";
}


export type Choice =
    | ChoiceAction
    | ChoiceReturn
    | ChoiceContinue;


export interface Log {
  number: number;
  text: string;
  effects: Array<Property>;
  chosen: Choice;
}

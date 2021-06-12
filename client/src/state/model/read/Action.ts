import type {Property} from "./Property";


export interface Requirement {
  display: string;
  passed: boolean;
}


export interface Option {
  urlname: string;
  text: string;
  icon: string | null;
  isAvailable: boolean;
  requirements: Array<Requirement>;
}


export interface Action {
  text: string;
  effects: Array<Property>;
  options: Array<Option>;
  hasReturn: boolean;
  hasContinue: boolean;
}

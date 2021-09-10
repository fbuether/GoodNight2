import type {Property} from "./Property";


export interface Requirement {
  display: string;
  icon?: string;
  passed: boolean;
}

export interface Test {
  display: string;
  icon?: string;
  chance: number;
}


export interface Option {
  urlname: string;
  text: string;
  icon: string | null;
  isAvailable: boolean;
  requirements: Array<Requirement>;
  tests: Array<Test>;
}


export interface Action {
  text: string;
  effects: Array<Property>;
  options: Array<Option>;
  hasReturn: boolean;
  hasContinue: boolean;
}

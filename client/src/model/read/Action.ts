import type {Property} from "./Property";


export interface Requirement {
  expression: object;
  passed: boolean;
}


export interface Option {
  text: string;
  icon: string | null;
  isAvailable: boolean;
  requirements: Array<Requirement>;
  effects: Array<Property>;
  scene: string;
}


export interface Action {
  text: string;
  effects: Array<Property>;
  options: Array<Option>;
  return: string | null;
  continue: string | null;
}

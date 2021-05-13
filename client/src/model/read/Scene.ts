import type {Quality} from "./Quality";
import type {Value} from "./Value";
import type {Property} from "./Property";


export interface Requirement {
  description: string;
  passed: boolean;
}


export interface Option {
  scene: string;
  isAvailable: boolean;
  text: string;
  requirements: Array<Requirement>;
}


export interface Scene {
  name: string;
  text: string;
  effects: Array<Property>;
  options: Array<Option>;
  return?: string;
  continue?: string;
}

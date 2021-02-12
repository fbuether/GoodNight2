import {Quality} from "./Quality.ts";
import {Value} from "./Value.ts";


interface Requirement {
  description: string;
  passed: boolean;
}


interface Option {
  scene: string;
  isAvailable: boolean;
  text: string;
  requirements: Array<Requirement>;
}


export interface Scene {
  urlname: string;
  text: string;
  effects: Array<[Quality, Value]>;
  options: Array<Option>;
  return: string;
  continue: string;
}

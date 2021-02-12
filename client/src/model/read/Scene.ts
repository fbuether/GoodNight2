
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


interface Scene {
  urlname: string;
  text: string;
  effects: Array<[Quality, Value]>;
  options: Array<Option>;
  return: string;
  continue: string;
}

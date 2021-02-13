import {Action} from "./Action";
import {Value} from "./Value";
import {Scene} from "./Scene";


export interface Player {
  user: string;
  story: string;
  name: string;
  history: Array<Action>;
  current: Scene;
  state: Map<string, Value>;
}


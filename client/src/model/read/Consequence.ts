import type {Action} from "./Action";
import type {Scene} from "./Scene";


export interface Consequence {
  action: Action;
  scene: Scene;
}

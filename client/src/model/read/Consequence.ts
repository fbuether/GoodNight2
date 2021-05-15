import type {Action} from "./Action";
import type {Log} from "./Log";


export interface Consequence {
  log: Log;
  action: Action;
}

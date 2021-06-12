import type {Player} from "./Player";
import type {Log} from "./Log";
import type {Action} from "./Action";


export interface Adventure {
  player: Player;
  history: Array<Log>;
  current: Action;
}

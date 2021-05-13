import type {Player} from "./Player";
import type {Story} from "./Story";
import type {Log} from "./Log";
import type {Action} from "./Action";


export interface Adventure {
  player: Player;
  user: string;
  story: Story;
  history: Array<Log>;
  current: Action;
}

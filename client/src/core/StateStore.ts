
import type {State} from "../state/State";
import {User} from "../state/User";
import {Home} from "../state/page/Home";


let stateRef: {state: State} = {
  state: {
    user: User.default,
    page: Home.page.state
  }
};


export const StateStore = {
  update: (update: (state: State) => State | null): State => {
    var updated = update(stateRef.state);
    if (updated == null) {
      return stateRef.state;
    }

    stateRef.state = updated;
    return stateRef.state;
  },

  get: () => stateRef.state
}

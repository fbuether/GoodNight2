import type {State} from "../state/State";


let stateRef: {state: State} = {
  state: ({
  } as State)
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

import State from "../model/State.ts";


export interface NoUpdate {
  kind: "no-update";
}


export type Update = NoUpdate;


export default function update(state: State, update: Update): State {
  return state;
}

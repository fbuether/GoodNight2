import type State from "../model/State";
import type {Page} from "../model/Page";
import NavigateAction from "./NavigateAction";


export interface NoUpdate {
  kind: "no-update";
}

export type Update = NoUpdate
    | NavigateAction;


function assertNever(param: never): never {
  throw new Error(`"update" received invalid state: "${param}"`);
}

export default function update(state: State, update: Update): State {
  switch (update.kind) {
    case "navigate": return update.execute(state); // navigateTo(state, update.page);
    case "no-update": return state;
    default: return assertNever(update);
  }
}

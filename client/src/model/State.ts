import * as O from "optics-ts";

import {Page} from "./Page";


export interface State {
  page: Page;
  user: string;
}

export const State = {
  lens: {
    ...O.optic<State>(),

    page: O.optic<State>().prop("page"),
    user: O.optic<State>().prop("user")
  },

  toUrl: (state: State): string => {
    return Page.toUrl(state.page);
  },

  ofUrl: (pathname: string): State => {
    // todo: load current user.
    return {
      page: Page.ofUrl(pathname),
      user: "Mrs. Hollywookle"
    };
  }
}


export type Update = (state: State) => State;

export function applyUpdate(state: State, update: Update): State {
  return update(state);
}

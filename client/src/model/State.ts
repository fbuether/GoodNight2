import * as O from "optics-ts";

import {goTo} from "../History";

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

  toTitle: (state: State): string => "GoodNight" + Page.toTitle(state.page),

  toUrl: (state: State): string => Page.toUrl(state.page),

  ofUrl: (pathname: string): State => {
    // todo: load current user.
    return {
      page: Page.ofUrl(pathname),
      user: "Mrs. Hollywookle"
    };
  },
}



export type Update = (state: State) => State;

export function applyUpdate(state: State, update: Update): State {
  let nextState = update(state);
  let nextUrl = State.toUrl(nextState);
  goTo(nextUrl);
  document.title = State.toTitle(nextState);
  return nextState;
}

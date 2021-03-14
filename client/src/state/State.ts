import * as P from "./ProtoLens";

import {Page} from "./Page";


export interface State {
  page: Page;
  user: string;
}

export interface WithState {
  state: Readonly<State>;
}


export const State = {
  lens: P.id<State>()
    .path("page", Page.lens)
    .prop("user"),

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


// performing updates to the state.

export type Update = (state: State) => State;
export type Dispatch = (action: Update) => void;

export function applyUpdate(state: State, update: Update): State {
  let nextState = update(state);
  let nextUrl = State.toUrl(nextState);
  goTo(nextUrl);
  document.title = State.toTitle(nextState);
  return nextState;
}


// treating the window.history object.

export function goTo(url: string, title: string = "GoodNight") {
  let lastUrl = history.state;
  if (lastUrl != url) {
    history.pushState(url, title, url);
  }
}


let registered = false;

export function registerHistoryListener(dispatch: (action: Update) => void) {
  if (registered) {
    return;
  }

  registered = true;

  window.addEventListener("popstate", (event: PopStateEvent) => {
    let restoredUrl = event.state;
    let restoredState = State.ofUrl(restoredUrl);
    let upd = (_: State) => restoredState;
    dispatch(upd);
  });
}

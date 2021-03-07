import {State, Update} from "./model/State";


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

// import * as P from "./ProtoLens";

// import {Dispatch, DispatchAction} from "./Dispatch";

import {PageState} from "./model/PageState";

import {User} from "./User";

// import {User} from "./User";

// import {Page} from "./Page";

// export interface State {
//   page: PageState;
//   user: null;
// }

// export interface WithState {
//   state: Readonly<State>;
// }



export interface State {
  page: PageState;
  user: User;
}


/*
export const State = {
  initial: (pathname: string) => {
    let descriptor = Page.ofUrl(pathname);
    let startState = {
      user: null,
      page: descriptor.state
    };

    return applyUpdate({dispatch:null})(startState, Dispatch.Page(descriptor));
  }
}
*/

// performing updates to the state.

/*
export function applyUpdate(r: { dispatch: Dispatch | null }) {
  return (state: State, update: DispatchAction): State => {
    console.log("applying update", update);

    switch (update.kind) {
      case "Update":

        var updatePage = update.update(state.page);
        var newstate = updatePage == null
            ? state
            : { ...state, page: updatePage };

        console.log("update: ", newstate);

        return newstate;
        // case "Command":
        //   return state;
      case "Page":
        let page = update.descriptor;
        let newState = { ...state, page: page.state };
        goTo(page.url, page.title);
        document.title = page.title;

        // setTimeout(() => r.dispatch( update.descriptor.onLoad(r.dispatch

        if (r.dispatch != null) {
          update.descriptor.onLoad(r.dispatch, newState);
        }
        else {
          console.warn("could not execute onLoad, as dispatch is null.");
        }

        return newState;

      // case "Async":
      //   return state;
    }

    throw new Error();
  }
}
*/

// // treating the window.history object.

// export function goTo(url: string, title: string = "GoodNight") {
//   let lastUrl = history.state;
//   if (lastUrl != url) {
//     history.pushState(url, title, url);
//   }
// }


// let registered = false;

// export function registerHistoryListener(dispatch: Dispatch) {
//   if (registered) {
//     return;
//   }

//   registered = true;

//   window.addEventListener("popstate", (event: PopStateEvent) => {
//     // console.log("hostiry");
//     // console.log(event.state, Page.ofUrl(event.state));
//     let restoredUrl = event.state;
//     let restoredPage = Page.ofUrl(restoredUrl);
//     dispatch(Dispatch.Page(restoredPage));
//   });
// }

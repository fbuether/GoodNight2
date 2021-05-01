import * as Preact from "preact";
// import * as PreactHooks from "preact/hooks";

// Bootstrap imports, filtered.

// Alert
// import "bootstrap/js/dist/button";
// Carousel
// import "bootstrap/js/dist/dropdown";
import "bootstrap/js/dist/collapse";
// Modal
// Popover
// ScrollSpy
// Tab
// Toast
// Tooltip

import "./ui/style.scss";

// import {// State, // applyUpdate,
//         registerHistoryListener} from "./state/State";

import {State} from "./state/State";

import {messages, dispatch, setExecutor, Dispatch, DispatchAction} from "./state/Dispatch";
import DispatchContext from "./DispatchContext";


import type {PageState} from "./state/model/PageState";
import type {PageDescriptor} from "./state/model/PageDescriptor";

import Page from "./components/Page";

import {Pages} from "./state/Page";
import {User} from "./state/User";
import {Home} from "./state/page/Home";

/*
let Root = () => {
  var reduceContainer: {dispatch: Dispatch | null} = { dispatch: null };
  var startState = State.initial(new URL(window.location.href).pathname);
  const [state, dispatch] = PreactHooks.useReducer(applyUpdate(reduceContainer), startState);
  reduceContainer.dispatch = dispatch;
  console.log("finished setting up reducer.");
  // dispatch(

  registerHistoryListener(dispatch);

  console.log("current state", state);

  return (
    <DispatchContext.Provider value={dispatch}>
      <Page {...state} />
    </DispatchContext.Provider>
  );
};
*/







let stateRef: {state: State} = {
  state: {
    user: User.default,
    page: Home.page.state
  }
};


function updateState(update: (state: State) => State | null): State {
  var updated = update(stateRef.state);
  if (updated == null) {
    return stateRef.state;
  }

  stateRef.state = updated;
  return stateRef.state;
};


let reRender = () => {
  let rootElement = document.getElementById("goodnight-client");
  if (rootElement == null) {
    throw "HTML does not contain the root element \"goodnight-client\".";
  }

  Preact.render(<Page {...stateRef.state} />, rootElement);
}




function executeNext() {
  if (messages.length == 0) {
    // console.log("no further messages.");
    return;
  }

  let msg = messages.shift();
  if (msg === undefined) {
    // console.log("executeNext: msg is undefined.");
    return;
  }

  console.log("executenext: ", msg);

  switch (msg.kind) {
    case "Command":
      msg.action();
      break;

    case "State":
      updateState(msg.action);
      break;

    case "Update":
      let upd = msg.update;
      updateState(state => {
        var updated = upd(state.page);
        return updated != null 
            ? { ...state, page: updated }
            : null;
      });
      break;

    case "Page":
      let desc = msg.descriptor;
      let newState = updateState(state => ({ ...state, page: desc.state }));
      document.title = desc.title;

      // must be prior to history push, as UserService must read a first url
      // that we have been redirected to.
      desc.onLoad(dispatch, newState);

      // history api.
      console.log("comparing history", history.state, desc.url);

      // console.log(`${history.state} == null || !${history.state.startsWith(desc.url)}`);
      // console.log(`${history.state == null || !history.state.startsWith(desc.url)}`);

      if (history.state != desc.url) {
        console.log("overriding state.");
        history.pushState(desc.url, desc.title, desc.url);
      }

      // setTimeout(() => // )
  ;
      // break;
  }

  if (messages.length == 0) {
    // console.log("executeNext: rerendering.");
    reRender();
  }
  else {
    // console.log("executeNext: one more msg.");
    executeNext();
  }
}




function ofUrl(pathname: string): PageDescriptor {
  let page = Pages.find(p => p.path.test(pathname));
  if (page !== undefined) {
    let matches = pathname.match(page.path);
    return page.ofUrl(pathname, matches != null ? matches : []);
  }

  return Home.page;
}


// link up to dispatch.
setExecutor(executeNext);

User.loadUser();


// history api
window.addEventListener("popstate", (event: PopStateEvent) => {
  // console.log("hostiry");
  // console.log(event.state, Page.ofUrl(event.state));

  console.log("history: popstate", event, ofUrl(event.state));
  let restoredUrl = event.state;
  let restoredPage = ofUrl(restoredUrl);
  dispatch(Dispatch.Page(restoredPage));
});


let url = new URL(window.location.href);

// // history api, again.
// var historyLocation = url.pathname + url.search + url.hash;
// history.pushState(historyLocation, document.title, url.pathname);
// console.log(`history.pushState(${historyLocation}, ${document.title}, ${url.pathname});`);

let initialDescriptor = ofUrl(url.pathname);
let initialDispatch = Dispatch.Page(initialDescriptor);
dispatch(initialDispatch);

import * as Preact from "preact";
import * as PreactHooks from "preact/hooks";

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

import {State, Update, applyUpdate,
  registerHistoryListener} from "./state/State";
import DispatchContext from "./DispatchContext";

import Page from "./components/Page";


let Root = () => {
  const [state, dispatch] = PreactHooks.useReducer<State, Update>(
    applyUpdate, State.ofUrl(new URL(window.location.href).pathname));

  registerHistoryListener(dispatch);

  console.log("current state", state);

  return (
    <DispatchContext.Provider value={dispatch}>
      <Page {...state}></Page>
    </DispatchContext.Provider>
  );
};

let rootElement = document.getElementById("goodnight-client");
if (rootElement == null) {
  throw "HTML does not contain the root element \"goodnight-client\".";
}

Preact.render(<Root />, rootElement);

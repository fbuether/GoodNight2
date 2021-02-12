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

import State from "./model/State";
import DispatchContext from "./DispatchContext";
import update, {Update} from "./update/Update";
import initialState from "./update/InitialState";

import Page from "./components/Page";


let Root = () => {
  const [state, dispatch] = PreactHooks.useReducer<State, Update>(
    update, initialState(new URL(window.location.href)));

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

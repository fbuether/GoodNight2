
import * as React from "react";
import * as ReactDOM from "react-dom";

// Bootstrap imports, filtered.
// Alert
import "bootstrap/js/dist/button";
// Carousel
import "bootstrap/js/dist/dropdown";
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
  const [state, dispatch] = React.useReducer<React.Reducer<State, Update>>(
    update, initialState);

  return (
    <DispatchContext.Provider value={dispatch}>
      <Page {...state}></Page>
    </DispatchContext.Provider>
  );
};

ReactDOM.render(<Root />,
  document.getElementById("goodnight-client"));

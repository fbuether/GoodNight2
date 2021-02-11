
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

import Page from "./Page";

let initial: State = {
  page: {
    kind: "start",
    message: "default message!"
  },
  user: "Mrs. Hollywookle"
};

ReactDOM.render(
  <Page {...initial}></Page>,
  document.getElementById("goodnight-client"));

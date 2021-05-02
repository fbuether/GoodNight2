import * as Preact from "preact";

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

import "./style.scss";


import {Dispatch} from "./core/Dispatch";
import {History} from "./core/History";
import {StateStore} from "./core/StateStore";

import {Page} from "./state/Page";
import {User} from "./state/User";

import PageComponent from "./components/Page";


User.loadUser();


// always render when updates finish.
Dispatch.onFinishUpdate(() => {
  let rootElement = document.getElementById("goodnight-client");
  if (rootElement == null) {
    throw "HTML does not contain the root element \"goodnight-client\".";
  }

  Preact.render(<PageComponent {...StateStore.get()} />, rootElement);
});


// whenever we go to a url (by opening the page, or via history), dispatch it
let gotoUrl = (url: string) => {
  let initialDescriptor = Page.ofUrl(url);
  let initialDispatch = Dispatch.Page(initialDescriptor);
  Dispatch.send(initialDispatch);
}

History.register(gotoUrl);

let initialUrl = new URL(window.location.href).pathname;
gotoUrl(initialUrl);

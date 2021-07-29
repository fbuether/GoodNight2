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


// explicit import required to have FinishSignIn not be pruned.
import {FinishSignIn} from "./state/page/user/FinishSignIn";
FinishSignIn;



// always render when updates finish.
Dispatch.onFinishUpdate(() => {
  let rootElement = document.getElementById("goodnight-client");
  if (rootElement == null) {
    throw "HTML does not contain the root element \"goodnight-client\".";
  }

  Preact.render(PageComponent(StateStore.get()), rootElement);
});

Dispatch.addPageValidator(Page.authCheck);

// whenever we go to a url (by opening the page, or via history), dispatch it
let gotoUrl = (url: string) => {
  Dispatch.send(Dispatch.Page(Page.ofUrl(url)));
}

History.register(gotoUrl);

let initialUrl = new URL(window.location.href).pathname;

// initially load the user prior to going to the first page. This is hopefully
// quick enough to not cause delays.
User.setInitialUser().then(user => {
  gotoUrl(initialUrl);
});

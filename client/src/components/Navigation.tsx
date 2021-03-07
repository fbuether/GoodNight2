import * as O from "optics-ts";

import {Update, State} from "../model/State";
import {Page} from "../model/Page";
import {HomePage} from "../model/HomePage";
import {WritePage} from "../model/write/WritePage";

import Link from "../components/common/Link";


export default interface Navigation {
  readonly currentPage: Page;
  readonly user: string;
}


export default function Navigation(state: State) {
  let navItems: Array<{title: string, target: State}> = [
    {
      title: "Home",
      target: O.set(State.lens.page)(HomePage.instance)(state)
    },
    // {
    //   title: "Read Stories",
    //   to: {
    //     kind: "read" as const,
    //     story: "Hels Schlucht",
    //     user: state.user
    //   }
    // },
    {
      title: "Write Stories",
      target: O.set(State.lens.page)(WritePage.instance)(state)
    },
  ];

  let activePage = { "aria-current": "page" };

  let currentKind = state.page.kind;
  console.log("current: ", currentKind);
  for (var p of navItems) {
    console.log("item:", p.target.page.kind, currentKind == p.target.page.kind);
  }

  let navButtons = navItems
    .map(item => ({...item, current: currentKind == item.target.page.kind}))
    .map(item => (
      <li class={"nav-item" + (item.current ? " active" : "")}>
        <Link class="nav-link" current={item.current} target={item.target}>
          {item.title}
        </Link>
      </li>));

  return (
    <nav class="navbar navbar-expand-sm navbar-light">
      <div class="container-fluid px-0">
        <span class="navbar-brand">GoodNight</span>

        <button class="navbar-toggler collapsed" type="button"
          data-bs-toggle="collapse" data-bs-target="#navbarNav"
          aria-controls="navbarNav" aria-expanded="false"
          aria-label="Toggle navigation">
          <span class="navbar-toggler-icon"></span>
        </button>

        <div class="collapse navbar-collapse" id="navbarNav">
          <ul class="navbar-nav me-auto">
            {navButtons}
          </ul>
        </div>
      </div>
    </nav>
  );
}

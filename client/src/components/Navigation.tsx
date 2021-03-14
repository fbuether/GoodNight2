
import {Update, State} from "../state/State";
import {Page} from "../state/Page";
import {HomePage} from "../state/HomePage";
import {ReadPage} from "../state/read/ReadPage";
import {WritePage} from "../state/write/WritePage";

import Link from "../components/common/Link";


export default interface Navigation {
  readonly state: Readonly<State>;
  readonly currentPage: Page;
  readonly user: string;
}

interface NavItem {
  title: string;
  page: Page;
}


export default function Navigation(state: Navigation) {
  let navItems: Array<NavItem> = [
    {
      title: "Willkommen",
      page: HomePage.instance
    },
    {
      title: "Geschichten lesen",
      page: ReadPage.instance
    },
    {
      title: "Geschichten schreiben",
      page: WritePage.instance
    },
  ];

  let activePage = { "aria-current": "page" };

  let currentKind = state.currentPage.kind;

  let navButtons = navItems
    .map(item => ({...item, current: currentKind == item.page.kind}))
    .map(item => (
      <li class={"nav-item" + (item.current ? " active" : "")}>
        <Link class="nav-link" current={item.current} state={state.state}
          target={State.lens.page.set(item.page)}>
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

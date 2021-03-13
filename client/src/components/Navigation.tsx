
import {Update, State} from "../state/State";
import {Page} from "../state/Page";
import {HomePage} from "../state/HomePage";
import {ReadPage} from "../state/read/ReadPage";
import {WritePage} from "../state/write/WritePage";

import Link from "../components/common/Link";


export default interface Navigation {
  readonly currentPage: Page;
  readonly user: string;
}


export default function Navigation(state: State) {
  let navItems: Array<{title: string, target: State}> = [
    {
      title: "Home",
      target: State.lens.page.set(HomePage.instance)(state)
    },
    {
      title: "Read Stories",
      target: State.lens.page.set(ReadPage.instance)(state)
    },
    {
      title: "Write Stories",
      target: State.lens.page.set(WritePage.instance)(state)
    },
  ];

  let activePage = { "aria-current": "page" };

  let currentKind = state.page.kind;

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

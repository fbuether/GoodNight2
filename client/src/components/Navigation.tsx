
import {Update, State} from "../state/State";
import {Page} from "../state/Page";
import {HomePage} from "../state/HomePage";
import {ReadPage} from "../state/read/ReadPage";
import {WritePage} from "../state/write/WritePage";

import {User} from "./user/User";

// import UserControl from "./UserControl";
import Link from "../components/common/Link";


type MenuItem = [string, { instance: Page }];

let menuItems: Array<MenuItem> = [
  ["Willkommen", HomePage],
  ["Geschichten lesen", ReadPage],
  ["Geschichten schreiben", WritePage]
];


interface NavButton {
  title: string;
  target: Update;
  state: State;
  current: boolean;
}


function NavItem(state: NavButton) {
  return (
    <li class={"nav-item" + (state.current ? " active" : "")}>
      <Link class="nav-link" current={state.current} state={state.state}
        target={state.target}>
        {state.title}
      </Link>
    </li>
  );
}


export default function Navigation(state: State) {

  let activePage = { "aria-current": "page" };

  let currentKind = state.page.kind;

  let buttons = menuItems.map(item =>
      <NavItem
        title={item[0]}
        target={State.lens.page.set(item[1].instance)}
        state={state}
        current={state.page.kind == item[1].instance.kind} />);

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
            {buttons}
          </ul>
          <ul class="navbar-nav">
            <li class="nav-item">
              <User.C {...User.instance} />
            </li>
          </ul>
        </div>
      </div>
    </nav>
  );
}

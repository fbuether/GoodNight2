// import {Dispatch} from "../../state/Dispatch";

import {PageDescriptor} from "../../state/model/PageDescriptor";

import {Home} from "../../state/page/Home";
import {StoryOverview} from "../../state/page/read/StoryOverview";

import {User} from "../user/User";
import {NavItem} from "./NavItem";

// import Link from "../common/Link";



type MenuItem = [string, PageDescriptor];

let menuItems: Array<MenuItem> = [
  ["Willkommen", Home.page],
  ["Geschichten lesen", StoryOverview.page],
  // ["Geschichten schreiben", WritePage]
];


export interface Navigation {
  user: User;
  page: string;
}

export function Navigation(state: Navigation) {
  let buttons = menuItems.map(item =>
    <NavItem
      title={item[0]}
      page={item[1]}
      current={state.page == item[1].state.page} />);

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
            <User {...state.user} />
          </ul>
        </div>
      </div>
    </nav>
  );
}

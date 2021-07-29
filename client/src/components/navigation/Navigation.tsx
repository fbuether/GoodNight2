import {PageDescriptor} from "../../core/PageDescriptor";

import {Navigation as State} from "../../state/Navigation";

import {NavItem} from "./NavItem";
import {User} from "../user/User";



export function Navigation(state: State) {
  let buttons = State.getMenuItems(state.user).map(item =>
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
            {User(state.user)}
          </ul>
        </div>
      </div>
    </nav>
  );
}

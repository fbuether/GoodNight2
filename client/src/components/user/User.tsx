import {User as State} from "../../state/User";

import Link from "../common/Link";
import Icon from "../common/Icon";


export function User(state: State) {
  if (state.kind == "SignedIn") {
    let abmeldenTitle = state.level == "guest"
        ? "Daten löschen"
        : "Abmelden";

    let userIcon = state.level == "guest"
        ? "ninja-head" as const
        : "astronaut-helmet" as const;

    return (
      <>
        <li class="nav-text">
          <Icon name={userIcon} class="small higher mr-1 restrained" />
          {state.name}
        </li>
        <li class="nav-item">
          <Link class="nav-link clickable"
            action={state.signOut}>
            <Icon name="crypt-entrance" class="small higher mr-1 restrained" />
            {abmeldenTitle}
          </Link>
        </li>
      </>
    );
  }
  else {
    return (
      <li class="nav-item">
        <Link class="nav-link clickable"
          action={state.signIn}>
          <Icon name="bookmarklet" class="small lower mr-1 restrained" />
          Anmelden
        </Link>
      </li>
    );
  }
}

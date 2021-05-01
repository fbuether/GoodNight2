import {dispatch, Dispatch} from "../../state/Dispatch";

import Link from "../common/Link";
import Icon from "../common/Icon";


type SignedIn = {
  kind: "SignedIn";
  name: string;
  signOut: () => Promise<void>;
}


type SignedOut = {
  kind: "SignedOut";
  signIn: () => Promise<void>;
}

export type User = SignedIn | SignedOut;


export function User(state: User) {
  if (state.kind == "SignedOut") {
    return (
      <li class="nav-item">
        <Link class="nav-link clickable"
          action={Dispatch.Command(state.signIn)}>
          <Icon name="bookmarklet" class="small lower mr-1 restrained" />
          Anmelden
        </Link>
      </li>
    );
  }
  else {
    return (
      <>
      <li class="nav-text">
        <Icon name="astronaut-helmet" class="small higher mr-1 restrained" />
        {state.name}
      </li>
      <li class="nav-item">
        <Link class="nav-link clickable"
          action={Dispatch.Command(state.signOut)}>
          <Icon name="crypt-entrance" class="small higher mr-1 restrained" />
          Abmelden
        </Link>
      </li>
      </>
    );
  }
}

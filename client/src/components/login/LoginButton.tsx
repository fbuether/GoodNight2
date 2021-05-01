
import type {State} from "../../state/State";


import Icon from "../common/Icon";
import Link from "../common/Link";
import Loading from "../common/Loading";


interface LoginButtonState {
  state: State;
  isLoading?: boolean;
}


function startLogin(event: MouseEvent) {
  event.preventDefault();

  // https://login.microsoftonline.com/common/oauth2/v2.0/authorize

}

export default function LoginButton(state: LoginButtonState) {
  state.isLoading = true;

  if (state.isLoading) {
    return <Loading />;
  }

  return (
    <a class="nav-link clickable" onClick={startLogin}>
      <Icon name="astronaut-helmet" class="small mr-1" />
      Login
    </a>
  );
}

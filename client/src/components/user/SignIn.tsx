
import Icon from "../common/Icon";


export interface SignIn {
  doSignIn: () => void;
}


function doSignIn(action: () => void) {
  return (event: MouseEvent) => {
    event.preventDefault();
    action();
  };
}


export const SignIn = {
  C: function(state: SignIn) {
    return (
      <a class="nav-link clickable" onClick={doSignIn(state.doSignIn)}>
        <Icon name="astronaut-helmet" class="small mr-1" />
        Anmelden
      </a>
    );
  }
}

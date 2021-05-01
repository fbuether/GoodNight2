
import Icon from "../common/Icon";


export interface SignOut {
  doSignOut: () => void;
}


function doSignOut(action: () => void) {
  return (event: MouseEvent) => {
    event.preventDefault();
    action();
  };
}


export const SignOut = {
  C: function(state: SignOut) {
    return (
      <a class="nav-link clickable" onClick={doSignOut(state.doSignOut)}>
        <Icon name="sundial" class="small mr-1" />
        Abmelden
      </a>
    );
  }
}

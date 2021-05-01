
import Icon from "../common/Icon";


export interface SignOut {
  
}


function doSignOut(action: () => void) {
  return (event: MouseEvent) => {
    event.preventDefault();
    action();
  };
}


export function SignOut(state: SignOut) {
  return (
    <Link class="nav-link clickable" action={
    <a class="nav-link clickable" onClick={doSignOut(state.doSignOut)}>
      <Icon name="sundial" class="small mr-1" />
      Abmelden
    </a>
  );
}


import type {RequireSignIn as State} from "../../state/page/user/RequireSignIn";


export function RequireSignIn(state: State) {
  return (
    <>Require to sign in to {state.target}</>
  );
}

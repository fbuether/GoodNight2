import * as Preact from "preact";
import * as PreactHooks from "preact/hooks";

import DispatchContext from "../../DispatchContext";

import {Page} from "../../state/Page";
import {DispatchAction} from "../../state/Dispatch";

// import {State, WithState, Update} from "../../state/State";
// import {Page} from "../../state/Page";



interface Link {
  readonly class?: string;

  readonly action: DispatchAction | string;

  readonly current?: boolean;
}

export default function Link(state: Preact.RenderableProps<Link>) {
  const dispatch = PreactHooks.useContext(DispatchContext);

  let dispatchLink = (action: DispatchAction) => (event: MouseEvent) => {
    event.preventDefault();
    dispatch(action);
  }

  if (typeof state.action === "string") {
    return <a class={state.class} href={state.action}>{state.children}</a>;
  }
  else {
    let href = // state.action
        // ? State.toUrl(state.target(state.state))
        // :
        "";

    return <a class={state.class}
      aria-current={state.current ? "page" : undefined}
      onClick={dispatchLink(state.action)}
      href={href}>{state.children}</a>;
  }
}

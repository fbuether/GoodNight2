import * as Preact from "preact";
import * as PreactHooks from "preact/hooks";

import {State, WithState, Update} from "../../state/State";
import {Page} from "../../state/Page";

import DispatchContext from "../../DispatchContext";


interface Link {
  readonly class?: string;

  readonly target: string | Update;

  readonly current?: boolean;
}

export default function Link(state: Preact.RenderableProps<Link & Partial<WithState>>) {
  const dispatch = PreactHooks.useContext(DispatchContext);

  let dispatchLink = (target: Update) => (event: MouseEvent) => {
    event.preventDefault();
    dispatch(target);
  }

  if (typeof state.target === "string") {
    return <a class={state.class} href={state.target}>{state.children}</a>;
  }
  else {
    let href = state.state
        ? State.toUrl(state.target(state.state))
        : "";

    return <a class={state.class}
      aria-current={state.current ? "page" : undefined}
      onClick={dispatchLink(state.target)}
      href={href}>{state.children}</a>;
  }
}

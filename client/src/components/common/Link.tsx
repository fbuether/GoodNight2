import * as Preact from "preact";
import * as PreactHooks from "preact/hooks";

import {State, Update} from "../../model/State";
import {Page} from "../../model/Page";

import DispatchContext from "../../DispatchContext";


interface Link {
  class?: string;
  current?: boolean;
  target: string | State;
}

export default function Link(state: Preact.RenderableProps<Link>) {
  const dispatch = PreactHooks.useContext(DispatchContext);

  let dispatchLink = (targetState: State) => (event: MouseEvent) => {
    event.preventDefault();
    dispatch(_ => targetState);
  }

  if (typeof state.target === "string") {
    return <a class={state.class} href={state.target}>{state.children}</a>;
  }
  else {
    return <a class={state.class}
      aria-current={state.current ? "page" : undefined}
      onClick={dispatchLink(state.target)}
      href={State.toUrl(state.target)}>{state.children}</a>;
  }
}

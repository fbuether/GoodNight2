import * as Preact from "preact";
import * as PreactHooks from "preact/hooks";

import {Page, asHref} from "../../model/Page";

import DispatchContext from "../../DispatchContext";
import {Update} from "../../update/Update";


interface Link {
  class?: string;
  to: Page | string;
}

function dispatchLink(dispatch: (u: Update) => void, page: Page) {
  return (event: MouseEvent) => {
    event.preventDefault();
    dispatch({kind: "Navigate", page: page });
  }
};

export default function Link(state: Preact.RenderableProps<Link>) {
  const dispatch = PreactHooks.useContext(DispatchContext);

  if (typeof state.to === "string") {
    return <a class={state.class} href={state.to}>{state.children}</a>;
  }
  else {
    return <a class={state.class}
      onClick={dispatchLink(dispatch, state.to)}
      href={asHref(state.to)}>{state.children}</a>;
  }
}

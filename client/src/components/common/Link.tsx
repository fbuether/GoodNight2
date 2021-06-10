import * as Preact from "preact";

import {Dispatch, DispatchAction} from "../../core/Dispatch";


interface Link {
  class?: string;
  action: DispatchAction | string; // string == regular link
  current?: boolean;
  title?: string;
}

export default function Link(state: Preact.RenderableProps<Link>) {
  let dispatchLink = (action: DispatchAction) => (event: MouseEvent) => {
    event.preventDefault();
    Dispatch.send(action);
  }

  if (typeof state.action === "string") {
    return <a class={state.class} href={state.action}>{state.children}</a>;
  }
  else {
    let href = state.action.kind == "Page"
        ? state.action.descriptor.url
        : "";

    return <a class={state.class}
      aria-current={state.current ? "page" : undefined}
      title={state.title}
      onClick={dispatchLink(state.action)}
      href={href}>{state.children}</a>;
  }
}

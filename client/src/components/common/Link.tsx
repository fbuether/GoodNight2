import * as Preact from "preact";

import {Dispatch, DispatchAction} from "../../core/Dispatch";


interface Link {
  readonly class?: string;
  readonly action: DispatchAction | string; // string == regular link
  readonly current?: boolean;
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
      onClick={dispatchLink(state.action)}
      href={href}>{state.children}</a>;
  }
}

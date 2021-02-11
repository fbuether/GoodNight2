import * as Preact from "preact";
import * as PreactHooks from "preact/hooks";

function linkHandler() {
  const currentProtocol = window.location.protocol;
  const currentHost = window.location.host;

  let links = document.getElementsByTagName("a");
  for (let link of Array.from(links)) {
    if (currentProtocol == link.protocol
        && currentHost == link.host) {
      let target = link.pathname;

      console.log("updating to navigate to ", target);
    }
  }
}


interface Link {
  href: string;
  class: string;
}



export default function Link(state: Preact.RenderableProps<Link>) {
  PreactHooks.useEffect(linkHandler);

  return (
    <a class={state.class} href={state.href}>{state.children}</a>
  );

}

import MD from "preact-markdown";

export default function Markdown(state: { children: string }) {
  return <p class="markdowned">{MD(state.children)}</p>;
}

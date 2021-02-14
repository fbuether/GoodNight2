import MD from "preact-markdown";

export default function Markdown(state: { text: string }) {
  return <p class="markdowned">{MD(state.text)}</p>;
}

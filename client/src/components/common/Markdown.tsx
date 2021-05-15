import MD from "preact-markdown";

export default function Markdown(state: { children: string }) {
  return MD(state.children);
}

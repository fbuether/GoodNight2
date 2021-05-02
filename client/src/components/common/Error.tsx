import * as Preact from "preact";

export interface Error {
  class?: string;
  message?: string;
}

export default function Error(state: Preact.RenderableProps<Error>) {
  var msg = state.message ?? "Ein Fehler ist aufgetreten.";
  return (
    <div class={"alert alert-danger alert-raw my-1 " + (state.class ?? "")}>
      {msg}
    </div>
  );
}

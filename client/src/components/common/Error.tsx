import * as Preact from "preact";

export interface Error {
  class?: string;
  message: string | null;
}

export default function Error(state: Preact.RenderableProps<Error>) {
  if (state.message === null) {
    return <></>;
  }

  return (
    <div class={"alert alert-danger alert-raw my-3 " + (state.class ?? "")}>
      {state.message}
    </div>
  );
}

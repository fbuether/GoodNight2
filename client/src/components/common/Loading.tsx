import * as Preact from "preact";

export interface LoadingState {
  class?: string;
}

export default function Loading(state: Preact.RenderableProps<LoadingState>) {
  return (
    <div class={"loading " + state.class ?? ""}>
      <div class="lined"></div>
      <div class="centre">Lade…</div>
      <div class="lined"></div>
    </div>
  );
}

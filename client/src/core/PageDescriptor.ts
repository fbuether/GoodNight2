import {VNode} from "preact";

import type {Pages} from "../state/Pages";
import type {State} from "../state/State";
import type {Dispatch} from "./Dispatch";

export interface PageDescriptor {
  state: Pages,
  url: string,
  title: string,

  // lifecycle
  onLoad: (dispatch: Dispatch, state: State) => Promise<void>;
}

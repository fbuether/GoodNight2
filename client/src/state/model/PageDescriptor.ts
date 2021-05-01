import {VNode} from "preact";

import type {PageState} from "./PageState";
import type {State} from "../State";
import type {Dispatch, DispatchAction} from "../Dispatch";

export interface PageDescriptor {
  state: PageState,
  url: string,
  title: string,

  // lifecycle
  onLoad: (dispatch: Dispatch, state: State) => Promise<void>;

  render: () => VNode<any>
}

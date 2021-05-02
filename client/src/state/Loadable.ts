import * as P from "../util/ProtoLens";

import {Dispatch} from "../core/Dispatch";
import {request, Method} from "../service/RequestService";
import type {State} from "./State";
import type {Pages} from "./Pages";

export type Unloaded = { state: "unloaded"; }
export type Loading = { state: "loading"; }
export type Loaded<T> = { state: "loaded"; result: T; }
export type Failed = { state: "failed"; error: string; }

export type Loadable<T> = Unloaded | Loading | Loaded<T> | Failed;


export const Loadable = {
  Unloaded: {
    state: "unloaded" as const
  },

  Loading: {
    state: "loading" as const
  },

  Loaded: <T>(data: T) => ({
    state: "loaded" as const,
    result: data
  }),

  Failed: (err: string) => ({
    state: "failed" as const,
    error: err
  }),

  forRequest: async <T>(dispatch: Dispatch, state: State, method: Method, url: string, lens: P.PrismAccess<Pages,Loadable<T>>) => {
    var loadable = lens.get(state.page);
    if (loadable == null) {
      return;
    }

    if (loadable.state == "unloaded") {
      dispatch(Dispatch.Update(lens.set(Loadable.Loading)));

      var response = await request<T>(method, url);

      dispatch(Dispatch.Update(lens.set(
        response.isResult
            ? Loadable.Loaded(response.message)
            : Loadable.Failed(response.message))));
    }
  }
};

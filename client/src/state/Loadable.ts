import * as P from "../util/ProtoLens";

import {Dispatch} from "../core/Dispatch";
import {request, Method} from "../service/RequestService";
import type {State} from "./State";
import type {Pages} from "./Pages";

export type Unloaded = { state: "unloaded"; }
export type UnloadedP<P> = { state: "unloadedP"; value: P; }
export type Loading = { state: "loading"; }
export type Loaded<T> = { state: "loaded"; result: T; }
export type Failed = { state: "failed"; error: string; }

export type Loadable<T> = Unloaded | Loading | Loaded<T> | Failed;
export type LoadableP<P,T> = UnloadedP<P> | Loading | Loaded<T> | Failed;

let guardUnloaded = <T>(a: Loadable<T>): a is Unloaded => (a.state == "unloaded");
let guardLoading = <T>(a: Loadable<T>): a is Loading => (a.state == "loading");
let guardLoaded = <T>(a: Loadable<T>): a is Loaded<T> => (a.state == "loaded");
let guardFailed = <T>(a: Loadable<T>): a is Failed => (a.state == "failed");

let guardUnloadedP = <P,T>(a: LoadableP<P,T>): a is UnloadedP<P> => (a.state == "unloadedP");
let guardLoadingP = <P,T>(a: LoadableP<P,T>): a is Loading => (a.state == "loading");
let guardLoadedP = <P,T>(a: LoadableP<P,T>): a is Loaded<T> => (a.state == "loaded");
let guardFailedP = <P,T>(a: LoadableP<P,T>): a is Failed => (a.state == "failed");


export const Loadable = {
  Unloaded: {
    state: "unloaded" as const
  },

  UnloadedP: <P>(value: P) => ({
    state: "unloadedP" as const,
    value: value
  }),

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


  lens: <S,T>(id: P.Prism<S, Loadable<T>>) => id
    .prop("state")
    .union("unloaded", guardUnloaded, lens => lens)
    .union("loading", guardLoading, lens => lens)
    .union("loaded", guardLoaded, lens => lens.prop("result"))
    .union("failed", guardFailed, lens => lens.prop("error")),

  lensP: <S,P,T>(id: P.Prism<S, LoadableP<P,T>>) => id
    .prop("state")
    .union("unloaded", guardUnloadedP, lens => lens.prop("value"))
    .union("loading", guardLoadingP, lens => lens)
    .union("loaded", guardLoadedP, lens => lens.prop("result"))
    .union("failed", guardFailedP, lens => lens.prop("error")),


  forRequest: async <T>(dispatch: Dispatch, state: State, method: Method,
    url: string, lens: P.PrismAccess<Pages,Loadable<T>>) => {

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
  },

  forRequestP: async <P,T>(dispatch: Dispatch, state: State, method: Method,
    url: (value: P) => string, lens: P.PrismAccess<Pages, LoadableP<P,T>>) => {

    var loadable = lens.get(state.page);
    if (loadable == null) {
      return;
    }

    if (loadable.state == "unloadedP") {
      dispatch(Dispatch.Update(lens.set(Loadable.Loading)));

      var response = await request<T>(method, url(loadable.value));
      dispatch(Dispatch.Update(lens.set(
        response.isResult
            ? Loadable.Loaded(response.message)
            : Loadable.Failed(response.message))));
    }
  }
};

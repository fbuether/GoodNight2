import * as P from "../util/ProtoLens";

import {Dispatch} from "../core/Dispatch";
import {request, Method, Response, makeResult, makeError} from "../service/RequestService";
import type {State} from "./State";
import type {Pages} from "./Pages";


export type Unloaded = { state: "unloaded"; }
export type UnloadedP<P> = { state: "unloadedP"; value: P; }
export type Loading<T> = { state: "loading"; request: Promise<Response<T>> };
export type Loaded<T> = { state: "loaded"; result: T; }
export type Failed = { state: "failed"; error: string; }

export type Loadable<T> = Unloaded | Loading<T> | Loaded<T> | Failed;
export type LoadableP<P,T> = UnloadedP<P> | Loading<T> | Loaded<T> | Failed;

let guardUnloaded = <T>(a: Loadable<T>): a is Unloaded => (a.state == "unloaded");
let guardLoading = <T>(a: Loadable<T>): a is Loading<T> => (a.state == "loading");
let guardLoaded = <T>(a: Loadable<T>): a is Loaded<T> => (a.state == "loaded");
let guardFailed = <T>(a: Loadable<T>): a is Failed => (a.state == "failed");

let guardUnloadedP = <P,T>(a: LoadableP<P,T>): a is UnloadedP<P> => (a.state == "unloadedP");
let guardLoadingP = <P,T>(a: LoadableP<P,T>): a is Loading<T> => (a.state == "loading");
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

  Loading: <T>(request: Promise<Response<T>>) => ({
    state: "loading" as const,
    request: request
  }),

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


  forRequest: async <T>(state: State, method: Method,
    url: string, lens: P.PrismAccess<Pages,Loadable<T>>)
    : Promise<null | Response<T>> => {

    var loadable = lens.get(state.page);
    if (loadable == null) {
      return null;
    }

    if (loadable.state == "unloaded") {
      var requestP = request<T>(method, url);
      Dispatch.send(Dispatch.Update(lens.set(Loadable.Loading(requestP))));
      var response = await requestP;

      Dispatch.send(Dispatch.Update(lens.set(
        response.isResult
            ? Loadable.Loaded(response.message)
            : Loadable.Failed(response.message))));

      return response;
    }

    return null;
  },

  forRequestP: async <P,T>(state: State, method: Method,
    url: (value: P) => string, lens: P.PrismAccess<Pages, LoadableP<P,T>>)
    : Promise<Response<T>> => {

    var loadable = lens.get(state.page);
    if (loadable == null) {
      return makeError(701, "Invalid state, loadable param not set");
    }

    switch (loadable.state) {
      case "unloadedP":
        var requestP = request<T>(method, url(loadable.value));
        Dispatch.send(Dispatch.Update(lens.set(Loadable.Loading(requestP))));
        var response = await requestP;

        Dispatch.send(Dispatch.Update(lens.set(
          response.isResult
              ? Loadable.Loaded(response.message)
              : Loadable.Failed(response.message))));
        return response;

      case "loading":
        var response = await loadable.request;
        Dispatch.send(Dispatch.Update(lens.set(
          response.isResult
              ? Loadable.Loaded(response.message)
              : Loadable.Failed(response.message))));
        return response;

      case "loaded":
        return makeResult<T>(loadable.result);

      case "failed":
        return makeError(702, loadable.error);
    }
  }
};

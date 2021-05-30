import {Dispatch} from "../../../core/Dispatch";
import type {PageDescriptor} from "../../../core/PageDescriptor";

import type {State} from "../../State";

import {Home} from "../Home";

import {User} from "../../User";
import {UserService} from "../../../service/UserService";



export interface FinishSignIn {
  page: "FinishSignIn"
}


async function onLoad(dispatch: Dispatch, state: State) {
  await UserService.get().finishSignIn();
  User.loadUser();

  Dispatch.send(Dispatch.Page(Home.page));
}


const instance = {
  page: "FinishSignIn" as const
};

const page: PageDescriptor = {
  state: instance,
  url: "/finish-sign-in",
  title: "GoodNight: Anmelden…",
  onLoad: onLoad
};

export const FinishSignIn = {
  path: /^\/finish-sign-in$/,
  // page: page,
  // instance: instance,
  ofUrl: (pathname: string, matches: Array<string>) => page// ,
  // render: SignInComponent
};

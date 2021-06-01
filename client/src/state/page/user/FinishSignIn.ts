import {Dispatch} from "../../../core/Dispatch";
import {PageDescriptor, registerPageMapper} from "../../../core/PageDescriptor";
import {Page} from "../../Page";

import type {State} from "../../State";

import {Home} from "../Home";

import {User} from "../../User";
import {UserService} from "../../../service/UserService";


export interface FinishSignIn {
  page: "FinishSignIn";
}


async function onLoad(dispatch: Dispatch, state: State) {
  var target = await UserService.get().finishSignIn();
  User.loadUser();

  let nextPage = target ? Page.ofUrl(target) : Home.page;
  Dispatch.send(Dispatch.Page(nextPage));
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


registerPageMapper(/^\/finish-sign-in$/, page);

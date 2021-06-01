import {Dispatch, DispatchAction} from "../../../core/Dispatch";
import {PageDescriptor, registerPageMapper} from "../../../core/PageDescriptor";
import {Page} from "../../Page";

import {User} from "../../User";
import {UserService} from "../../../service/UserService";

import {Home} from "../Home";


export interface RequireSignIn {
  page: "RequireSignIn";
  target: string;
  signInGuest: DispatchAction;
  signInUser: DispatchAction;
}


function signInGuest(target: string) {
  return async () => {
    UserService.get().createNewGuest();
    User.loadUser();

    console.log("now:", await UserService.get().getUser());

    let nextPage = target ? Page.ofUrl(target) : Home.page;
    Dispatch.send(Dispatch.Page(nextPage));
  }
}

function signInUser(target: string): (() => Promise<void>) {
  return async () => {
    return UserService.get().startSignIn(target);
  }
}


function instance(target: string): RequireSignIn {
  return {
    page: "RequireSignIn" as const,
    target: target,
    signInGuest: Dispatch.Command(signInGuest(target)),
    signInUser: Dispatch.Command(signInUser(target))
  };
}


function page(target: string): PageDescriptor {
  return {
    state: instance(target),
    url: "/sign-in/to" + target,
    title: "GoodNight: Anmelden…"
  };
}


export const RequireSignIn = {
  forUrl: (target: string) => page(target)
}

registerPageMapper(/^\/sign-in\/to(\/.+)$/,
  (matches: ReadonlyArray<string>) => page(matches[1]));

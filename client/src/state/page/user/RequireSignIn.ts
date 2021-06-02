import {Dispatch, DispatchAction} from "../../../core/Dispatch";
import {PageDescriptor, registerPageMapper} from "../../../core/PageDescriptor";

import {User} from "../../User";
import {UserService} from "../../../service/UserService";

import {Home} from "../Home";


export interface RequireSignIn {
  page: "RequireSignIn";
  target: string;
  signInGuest: DispatchAction;
  signInUser: DispatchAction;
}

type Resolver = (path: string) => PageDescriptor;


function signInGuest(target: string, resolver: Resolver) {
  return async () => {
    UserService.get().createNewGuest();
    User.loadUser();

    console.log("now:", await UserService.get().getUser());

    let nextPage = target ? resolver(target) : Home.page;
    Dispatch.send(Dispatch.Page(nextPage));
  }
}

function signInUser(target: string): (() => Promise<void>) {
  return async () => {
    return UserService.get().startSignIn(target);
  }
}


function instance(target: string, resolver: Resolver): RequireSignIn {
  return {
    page: "RequireSignIn" as const,
    target: target,
    signInGuest: Dispatch.Command(signInGuest(target, resolver)),
    signInUser: Dispatch.Command(signInUser(target))
  };
}


function page(target: string, resolver: Resolver): PageDescriptor {
  return {
    state: instance(target, resolver),
    url: "/sign-in/to" + target,
    title: "GoodNight: Anmelden…"
  };
}



export const RequireSignIn = {
  forUrl: (target: string, resolver: Resolver) => page(target, resolver)
}

registerPageMapper(/^\/sign-in\/to(\/.+)$/,
  (matches: ReadonlyArray<string>) => page(matches[1], () => Home.page));

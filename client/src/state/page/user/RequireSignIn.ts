import {Dispatch} from "../../../core/Dispatch";
import {PageDescriptor, registerPageMapper} from "../../../core/PageDescriptor";

import {User} from "../../User";
import {UserService} from "../../../service/UserService";


export interface RequireSignIn {
  page: "RequireSignIn";
  target: string;
  signInGuest: () => Promise<void>;
  signInUser: () => Promise<void>;
}


async function signInGuest() {
  UserService.get().createNewGuest();
  // Dispatch.send(Dispatch.Page(

  console.log("siginin in as guest");
}

async function signInUser() {
  console.log("siginin in as user");
}


function instance(target: string): RequireSignIn {
  return {
    page: "RequireSignIn" as const,
    target: target,
    signInGuest: signInGuest,
    signInUser: signInUser
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

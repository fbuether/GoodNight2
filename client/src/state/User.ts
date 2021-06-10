import {Dispatch, DispatchAction} from "../core/Dispatch";

import {UserService} from "../service/UserService";


type SignedIn = {
  kind: "SignedIn";
  name: string;
  level: "guest" | "bearer";
  signOut: DispatchAction;
}


type SignedOut = {
  kind: "SignedOut";
  signIn: DispatchAction;
}

export type User = SignedIn | SignedOut;


function setUser(user: User) {
  Dispatch.send(Dispatch.State(state => ({
    ...state,
    user: user
  })));
}

async function loadUser() {
  let user = await UserService.get().getUser();

  if (user != null) {
    var userState = signedInUser(user.level, user.email ?? user.name);
    setUser(userState);
  }
}


async function setInitialUser() {
  setUser(defaultUser);
  await loadUser();
}

async function removeUser() {
  await UserService.get().removeUser();
  setUser(defaultUser);

  // todo: check if we need to redirect to somewhere else, if the current page
  // is not valid without a user.
}



function signedInUser(level: "guest" | "bearer", name: string): User {
  return {
    kind: "SignedIn" as const,
    level: level,
    name: name,
    signOut: Dispatch.Command(removeUser)
  };
}

const defaultUser: User = {
  kind: "SignedOut" as const,
  signIn: Dispatch.Command(() => UserService.get().startSignIn(
    new URL(window.location.href).pathname))
};

export const User = {
  default: defaultUser,
  loadUser: loadUser,
  setInitialUser: setInitialUser,
  signedInUser: signedInUser
}

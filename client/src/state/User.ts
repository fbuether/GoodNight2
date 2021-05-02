import {Dispatch} from "../core/Dispatch";

import {User as UserState} from "../components/user/User";
import {UserService} from "../service/UserService";



export type User = UserState;


async function loadUser() {
  let user = await UserService.get().getUser();

  if (user != null) {
    var userState = signedInUser(user.email ?? user.name);

    Dispatch.send(Dispatch.State(state => ({
      ...state,
      user: userState
    })));
  }
}


async function setInitialUser() {
  Dispatch.send(Dispatch.State(state => ({
    ...state,
    user: defaultUser
  })));

  await loadUser();
}

async function removeUser() {
  await UserService.get().removeUser();
  Dispatch.send(Dispatch.State(state => ({
    ...state,
    user: defaultUser
  })));
}



function signedInUser(name: string): User {
  return {
    kind: "SignedIn" as const,
    name: name,
    signOut: () => removeUser()
  };
}

const defaultUser: User = {
  kind: "SignedOut" as const,
  signIn: () => UserService.get().startSignIn()
};

export const User = {
  default: defaultUser,
  loadUser: loadUser,
  setInitialUser: setInitialUser,
  signedInUser: signedInUser
}

import {dispatch, Dispatch} from "./Dispatch";

import {User as UserState} from "../components/user/User";


import {UserService} from "../service/UserService";


// var service = new UserService();



export type User = UserState;


async function loadUser() {
  let user = await UserService.get().getUser();

  console.log("user:", user);

  if (user != null) {
    var userState = signedInUser(user.email ?? user.name);

    dispatch(Dispatch.State(state => ({
      ...state,
      user: userState
    })));
  }
}

async function removeUser() {
  await UserService.get().startSignOut();
  dispatch(Dispatch.State(state => ({
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
  signedInUser: signedInUser
}


// import Oidc from "oidc-client";
// import * as P from "./ProtoLens";


// // export interface NoUser {
// //   kind: "NoUser";
// // }

// // export interface AUser {
// //   kind: "AUser";
// //   id: string;
// //   name: string;
// // }

// export type User = {
//   oidc: Oidc.UserManager | null;
// };


// // export const NoUser = {
// //   instance: {
// //     kind: "NoUser" as const
// //   },

// //   lens: <T>(id: P.Prism<T, NoUser>) => id
// // }


// // export const AUser = {
// //   instance: {
// //     kind: "AUser" as const
// //   },

// //   lens: <T>(id: P.Prism<T, AUser>) => id
// // }


// // let guardNoUser = (a: User): a is NoUser => (a.kind == "NoUser");
// // let guardAUser = (a: User): a is AUser => (a.kind == "AUser");

// export const User = {
//   instance: () => ({
//     oidc: null
//   })
//   //   return {
//   //     oidc: new Oidc.UserManager({
//   //       "authority": "https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration",
//   //       "client_id": "d8ee60f3-f059-4169-93b4-8faf1c32a9d8",
//   //       "redirect_uri": "http://localhost:32015/"
//   //     }),
//   //     ...NoUser.instance
//   //   };
//   // },

//   // lens: <T>(id: P.Lens<T, User>) => id
//     // .union("noUser", guardNoUser, NoUser.lens)
//     // .union("aUser", guardAUser, AUser.lens),
// }

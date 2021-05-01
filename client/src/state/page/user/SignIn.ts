import {Dispatch} from "../../Dispatch";
import type {State} from "../../State";
// import type {PageState} from "../../model/PageState";
// import type {PageDescriptor} from "../../model/PageDescriptor";
// import {Lens} from "../../model/PageState";

import type {PageDescriptor} from "../../model/PageDescriptor";
import {SignIn as SignInComponent} from "../../../components/page/user/SignIn";
import {Home} from "../Home";

import {User} from "../../User";
import {UserService} from "../../../service/UserService";



export type SignIn = SignInComponent;


async function onLoad(dispatch: Dispatch, state: State) {
  console.log("finishing sign-in.");

  var res = await UserService.get().finishSignIn();
  console.log("finished sign in", res);

  User.loadUser();

  // var user = await UserService.get().getUser();
  // console.log("user:", user);

  // dispatch(Dispatch.State(state => ({
  //   ...state,
  //   user: User.signedInUser()
  // })));

  dispatch(Dispatch.Page(Home.page));
}


const instance = {
  page: "SignIn" as const
};

const page: PageDescriptor = {
  state: instance,
  url: "/finish-sign-in",
  title: "GoodNight: Anmelden…",
  onLoad: onLoad,
  render: () => SignInComponent(instance)
};

export const SignIn = {
  path: /^\/finish-sign-in$/,
  // page: page,
  // instance: instance,
  ofUrl: (pathname: string, matches: Array<string>) => page// ,
  // render: SignInComponent
};

import Oidc from "oidc-client";

import useAsyncEffect from "../ui/useAsyncEffect";

import {State} from "../state/State";
import {User as UserState} from "../state/User";



import LoginButton from "./login/LoginButton";


// function Login(state: { state: State, user: NoUser }) {
//   return (<>Login!</>);
// }


// function User(state: { state: State, user: AUser }) {
//   return (<>Hello, {state.user.name}</>);
// }


function assertNever(param: never): never {
  throw new Error(`Invalid kind in UserControl: "${param}"`);
}

export default function UserControl(param: { state: State, user: UserState }) {
  let state = param.state;
  let user = param.user;


  // Oidc.Log.logger = console;

  // var um = new Oidc.UserManager({
  //   "authority": "https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration",
  //   "client_id": "d8ee60f3-f059-4169-93b4-8faf1c32a9d8",
  //   "redirect_uri": "http://localhost:32015/"
  // });

  // console.log(um);

  //   useAsyncEffect(
  //     async() => {
  //       var test = await um.signinRedirectCallback();

  //       console.log("response:", test);

  //       var user = await um.getUser();
  //       console.log(await um.getUser());
  //       // if (user == null) {
  //       //   await um.signinRedirect();
  //       // }

  //   });



  // switch (user.kind) {
  //   case "NoUser":
      return <LoginButton state={state} />;
    // case "AUser": return <User state={state} user={user} />;
    // default: assertNever(user.kind);
  // }

  // return <>something is wrong</>;
}

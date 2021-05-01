// import {createElement} from "preact";
import * as PreactHooks from "preact/hooks";
// import {UserService, User as UserData} from "../../service/UserService";
import DispatchContext from "../../DispatchContext";
// import useAsyncEffect from "../../ui/useAsyncEffect";

// import {State, Dispatch} from "../../state/State";

import {SignIn} from "./SignIn";
import {SignOut} from "./SignOut";

// import Loading from "../common/Loading";


export interface User {
  user: UserData | null;
}


export function User(state: User) {
  const dispatch = PreactHooks.useContext(DispatchContext);

  var user = state.user;
  if (user == null) {
    return (
      <SignIn.C doSignIn={dispatch("doSignIn")} />
    );
  }
  else {
    return (
      <>
        {state.user.name}
        <SignOut.C doSignOut={dispatch("doSignOut")} />
      </>
    );
  }
}


// // type SignInMessage = {
// //   kind: "SignInMessage";
// //   user: UserData;
// // }

// // let f: never = createElement("div", {}, "okay");


// export const User = {
//   instance: {
//     // service: UserService.get(),
//     // userLoaded: false,
//     user: null
//   },


//   // ofUrl: (pathname: string, matches: Array<string>): JSX.VNode<any> => {

//   //   return (<Loading />);
//   // },


//   C: (state: User) => {
//     const dispatch = PreactHooks.useContext(DispatchContext);

//     if (!state.userLoaded) {
//       // dispatch(

//       // useAsyncEffect(async() => {
//       //   let user = await state.service.getUser();
//       //   console.log("user", user);
//       // });
//     }

//     let doSignIn = () => {
//       state.service.startSignIn();
//       // dispatch({kind: "SignInMessage" as const});
//     };

//     let doSignOut = () => {
//       state.service.startSignOut();
//     }


//     if (state.user != null) {
//       return <>{state.user.name}<SignOut.C doSignOut={doSignOut} /></>;
//     }
//     else {
//       return <SignIn.C doSignIn={doSignIn} />;
//     }
//   }
// }


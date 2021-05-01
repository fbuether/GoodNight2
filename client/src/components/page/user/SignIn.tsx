import * as PreactHooks from "preact/hooks";
// import DispatchContext from "../../../DispatchContext";
import {Dispatch} from "../../../state/Dispatch";

import Loading from "../../common/Loading";


export interface SignIn {
  page: "SignIn"
}


// export const SignIn: PageStruct<SignIn> = {
//   path: /^finish-sign-in$/,

//   ofUrl: (pathname: string, matches: Array<string>) => ({
//     page: "SignIn" as const
//   }),

//   render: (state: SignIn) => {
//     return (<Loading />);
//   }
// };


export function SignIn(state: SignIn) {
  // const dispatch = PreactHooks.useContext(DispatchContext);
  // dispatch(Dispatch.Command("FinishSignIn"));

  return (
    <Loading />
  );
}

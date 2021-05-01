import {PageDescriptor} from "../../model/PageDescriptor";
import {SignIn as SignInComponent} from "../../../components/page/user/SignIn";

export type SignIn = SignInComponent;


const instance = {
  page: "SignIn" as const
};

const page: PageDescriptor = {
  state: instance,
  url: "/finish-sign-in",
  title: "GoodNight: Anmelden…",
  onLoad: () => Promise.resolve(),
  render: () => SignInComponent(instance)
};

export const SignIn = {
  path: /^\/finish-sign-in$/,
  // page: page,
  // instance: instance,
  ofUrl: (pathname: string, matches: Array<string>) => page// ,
  // render: SignInComponent
};

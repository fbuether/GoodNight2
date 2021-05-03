import {PageDescriptor} from "../../core/PageDescriptor";
import {Home as Component} from "../../pages/Home";

export type Home = Component;


const instance = {
  page: "Home" as const
};

const page: PageDescriptor = {
  state: instance,
  url: "/",
  title: "GoodNight",
  onLoad: () => Promise.resolve()
};

export const Home = {
  path: /^$/,
  page: page,
  // instance: instance,
  ofUrl: (pathname: string, matches: Array<string>) => page// ,
  // render: Component
};

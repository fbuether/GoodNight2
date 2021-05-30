import {PageDescriptor, registerPageMapper} from "../../core/PageDescriptor";
import {Home as Component} from "../../pages/Home";

export type Home = Component;


const instance = {
  page: "Home" as const
};

const page: PageDescriptor = {
  state: instance,
  url: "/",
  title: "GoodNight"
};

export const Home = {
  page: page
};


registerPageMapper(/^$/, page);

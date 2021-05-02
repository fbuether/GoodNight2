import * as P from "../util/ProtoLens";

import {Home} from "./page/Home";
import {SignIn} from "./page/user/SignIn";
import {StoryOverview} from "./page/read/StoryOverview";

import type {Page as PageState} from "../components/Page";
import type {Pages} from "./Pages";

import {PageDescriptor} from "../core/PageDescriptor";


export type Page = PageState;


export const Page = {

  Pages: [
    Home,
    SignIn,
    StoryOverview],

  default: Home.page,


  ofUrl: (pathname: string): PageDescriptor => {
    let page = Page.Pages.find(p => p.path.test(pathname));
    if (page !== undefined) {
      let matches = pathname.match(page.path);
      return page.ofUrl(pathname, matches != null ? matches : []);
    }

    return Home.page;
  }
}

// import * as P from "./ProtoLens";
// import {JSXInternal} from "preact";

// import {OfUrl, WithPath} from "../util/UrlMapper";

// import {HomePage} from "./HomePage";
// import {ReadPage} from "./read/ReadPage";
// import {WritePage} from "./write/WritePage";


// import type {PageState, PageStruct} from "./PageStruct";

import {Home} from "./page/Home";
import {SignIn} from "./page/user/SignIn";
import {StoryOverview} from "./page/read/StoryOverview";

import {PageState} from "./model/PageState";

import {PageDescriptor} from "./model/PageDescriptor";

// export type Page =
//     | HomePage
//     | ReadPage
//     // | LoginPage
//     | WritePage;


// export interface Page {
//   render: () => any;
// }

// export type Page = PageModel;
    // | Home
    // | SignIn
    // | StoryOverview;



export const Pages = [
  Home,
  SignIn,
  StoryOverview];

export const defaultPage = Home.page;




// function assertNever(param: never): never {
//   throw new Error(`Invalid Page kind in state Page: "${param}"`);
// }

// let guardHomePage = (a: Page): a is HomePage => (a.kind == "HomePage");
// let guardReadPage = (a: Page): a is ReadPage => (a.kind == "ReadPage");
// let guardWritePage = (a: Page): a is WritePage => (a.kind == "WritePage");

export const Page = {
  // lens: <T>(id: P.Lens<T, Page>) => id
  //   .union("home", guardHomePage, HomePage.lens)
  //   .union("read", guardReadPage, ReadPage.lens)
  //   .union("write", guardWritePage, WritePage.lens),

  // toUrl: (page: Page): string => {
  //   switch (page.kind) {
  //     case "HomePage": return HomePage.toUrl(page);
  //     case "ReadPage": return ReadPage.toUrl(page);
  //     case "WritePage": return WritePage.toUrl(page);
  //     default: return assertNever(page);
  //   }
  // },

  // ofUrl: (pathname: string): Page => {
  //   return OfUrl.union(pathname, [HomePage, ReadPage, WritePage],
  //     HomePage.instance);
  // },

  // toTitle: (page: Page): string => {
  //   switch (page.kind) {
  //     case "HomePage": return "";
  //     case "ReadPage": return ": " + ReadPage.toTitle(page);
  //     case "WritePage": return ": " + WritePage.toTitle(page);
  //     default: return assertNever(page);
  //   }
  // }


  ofUrl: (pathname: string): PageDescriptor => {
    let page = Pages.find(p => p.path.test(pathname));
    if (page !== undefined) {
      let matches = pathname.match(page.path);
      return page.ofUrl(pathname, matches != null ? matches : []);
    }

    return defaultPage;
  }
}

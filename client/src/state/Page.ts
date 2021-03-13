import * as P from "./ProtoLens";

import {HomePage} from "./HomePage";
import {ReadPage} from "./read/ReadPage";
import {WritePage} from "./write/WritePage";


export type Page =
    | HomePage
    | ReadPage
    // | LoginPage
    | WritePage;


function assertNever(param: never): never {
  throw new Error(`Invalid Page kind in state Page: "${param}"`);
}

let guardHomePage = (a: Page): a is HomePage => (a.kind == "HomePage");
let guardReadPage = (a: Page): a is ReadPage => (a.kind == "ReadPage");
let guardWritePage = (a: Page): a is WritePage => (a.kind == "WritePage");

export const Page = {
  lens: <T>(id: P.Lens<T, Page>) => id
    .union("home", guardHomePage, HomePage.lens)
    .union("read", guardReadPage, ReadPage.lens)
    .union("write", guardWritePage, WritePage.lens),

  toUrl: (page: Page): string => {
    switch (page.kind) {
      case "HomePage": return HomePage.toUrl(page);
      case "ReadPage": return ReadPage.toUrl(page);
      case "WritePage": return WritePage.toUrl(page);
      default: return assertNever(page);
    }
  },

  ofUrl: (pathname: string): Page => {
    let pages = [HomePage, ReadPage, WritePage];
    let page = pages.find(p => p.path.test(pathname));
    return page !== undefined
        ? page.ofUrl(pathname)
        : HomePage.instance;
  },

  toTitle: (page: Page): string => {
    switch (page.kind) {
      case "HomePage": return "";
      case "ReadPage": return ": " + ReadPage.toTitle(page);
      case "WritePage": return ": " + WritePage.toTitle(page);
      default: return assertNever(page);
    }
  }
}

import * as O from "optics-ts";

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


export const Page = {
  lens: {
    ...O.optic<Page>(),

    homePage: O.optic<Page>().guard(HomePage.guard),
  },

  toUrl: (page: Page): string => {
    switch (page.kind) {
      case "HomePage": return HomePage.toUrl(page);
      case "ReadPage": return ReadPage.toUrl(page);
      case "WritePage": return WritePage.toUrl(page);
      default: return assertNever(page);
    }
  },

  ofUrl: (pathname: string): Page => {
    let pages = [HomePage, WritePage];
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

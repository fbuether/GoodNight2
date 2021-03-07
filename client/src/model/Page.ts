import * as O from "optics-ts";

import {HomePage} from "./HomePage";
// import type {SelectStoryReadPage} from "./read/SelectStoryReadPage";
// import type {ReadPage} from "./read/ReadPage";
import {WritePage} from "./write/WritePage";


export type Page =
    | HomePage
    // | SelectStoryPage
    // | ReadPage
    // | LoginPage
    | WritePage;


export const Page = {
  lens: {
    ...O.optic<Page>(),

    homePage: O.optic<Page>().guard(HomePage.guard),
  },

  toUrl: (page: Page): string => {
    switch (page.kind) {
      case "HomePage": return HomePage.toUrl(page);
      case "WritePage": return WritePage.toUrl(page);
    }
  },

  ofUrl: (pathname: string): Page => {
    let pages = [HomePage, WritePage];
    let page = pages.find(p => p.path.test(pathname));
    return page !== undefined
        ? page.ofUrl(pathname)
        : HomePage.instance;
  }
}

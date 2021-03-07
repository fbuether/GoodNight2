import * as O from "optics-ts";

import {Page} from "../Page.ts";
// import {SelectStoryPart} from "./SelectStoryPart";
// import {WriteStoryPart} from "./WriteStoryPart";


// export type WritePagePart =
//     | SelectStoryPart
//     | WriteStoryPart;

export interface WritePage {
  kind: "WritePage";
  // part: WritePagePart;
}


export const WritePage = {
  instance: {
    kind: "WritePage" as const
  },

  toTitle: (page: WritePage) => "Write",

  guard: (a: Page): a is WritePage => (a.kind == "WritePage"),

  path: /^\/write/,

  toUrl: (homePage: WritePage): string => "/write",

  ofUrl: (pathname: string): WritePage => WritePage.instance
}

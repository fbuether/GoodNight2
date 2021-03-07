import * as O from "optics-ts";

import {Page} from "../Page.ts";
import {SelectStoryPart} from "./SelectStoryPart";
import {ReadStoryPart} from "./ReadStoryPart";


export type ReadPagePart =
    | SelectStoryPart
    | ReadStoryPart;

export interface ReadPage {
  kind: "ReadPage";
  part: ReadPagePart;
}


export const ReadPage = {
  instance: {
    kind: "ReadPage" as const,
    part: SelectStoryPart.instance
  },

  toTitle: (page: ReadPage) => "Read",

  guard: (a: Page): a is ReadPage => (a.kind == "ReadPage"),

  path: /^\/read/,

  toUrl: (homePage: ReadPage): string => "/read",

  ofUrl: (pathname: string): ReadPage => ReadPage.instance
}

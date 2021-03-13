import * as P from "../ProtoLens";

import {SelectStoryPart} from "./SelectStoryPart";
import {ReadStoryPart} from "./ReadStoryPart";


export type ReadPagePart =
    | SelectStoryPart
    | ReadStoryPart;


export interface ReadPage {
  kind: "ReadPage";
  part: ReadPagePart;
}


let guardSelectStoryPart = (a: ReadPagePart): a is SelectStoryPart =>
    (a.kind == "SelectStoryPart");
let guardReadStoryPart = (a: ReadPagePart): a is ReadStoryPart =>
    (a.kind == "ReadStoryPart");


export const ReadPage = {
  instance: {
    kind: "ReadPage" as const,
    part: SelectStoryPart.instance
  },

  lens: <T>(id: P.Prism<T,ReadPage>) => id
    .path("part", lens => lens
      .union("selectStory", guardSelectStoryPart, SelectStoryPart.lens)
      .union("readStory", guardReadStoryPart, ReadStoryPart.lens)),

  update: {
    part: (part: ReadPagePart) => (readPage: ReadPage) => ({ ...readPage, part: part }),
  },

  toTitle: (page: ReadPage) => "Read",

  path: /^\/read/,

  toUrl: (homePage: ReadPage): string => "/read",

  ofUrl: (pathname: string): ReadPage => ReadPage.instance
}

import * as P from "../ProtoLens";

import {OfUrl} from "../../util/UrlMapper";

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


function assertNever(param: never): never {
  throw new Error(`Invalid Page kind in state ReadPage: "${param}"`);
}

export const ReadPage = {
  instance: {
    kind: "ReadPage" as const,
    part: SelectStoryPart.instance
  },

  lens: <T>(id: P.Prism<T,ReadPage>) => id
    .path("part", lens => lens
      .union("selectStory", guardSelectStoryPart, SelectStoryPart.lens)
      .union("readStory", guardReadStoryPart, ReadStoryPart.lens)),

  toTitle: (page: ReadPage) => "Read",

  path: /^\/read(.+)?$/,

  toUrl: (page: ReadPage): string => {
    switch (page.part.kind) {
      case "SelectStoryPart": return "/read";
      case "ReadStoryPart": return "/read" + ReadStoryPart.toUrl(page.part);
      default: return assertNever(page.part);
    }
  },

  ofUrl: (pathname: string, matches: Array<string>): ReadPage => ({
    kind: "ReadPage" as const,
    part: OfUrl.union(matches[1],
      [SelectStoryPart, ReadStoryPart],
      SelectStoryPart.instance)
  })
}

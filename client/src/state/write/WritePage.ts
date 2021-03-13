import * as P from "../ProtoLens";

import {SelectStoryPart} from "./SelectStoryPart";
import {WriteStoryPart} from "./WriteStoryPart";


export type WritePagePart =
    | SelectStoryPart
    | WriteStoryPart;

export interface WritePage {
  kind: "WritePage";
  part: WritePagePart;
}


let guardSelectStoryPart = (a: WritePagePart): a is SelectStoryPart =>
    (a.kind == "SelectStoryPart");
let guardWriteStoryPart = (a: WritePagePart): a is WriteStoryPart =>
    (a.kind == "WriteStoryPart");


export const WritePage = {
  instance: {
    kind: "WritePage" as const,
    part: {
      kind: "SelectStoryPart" as const,
      stories: null
    }
  },

  lens: <T>(id: P.Prism<T, WritePage>) => id
    .path("part", lens => lens
      .union("selectStory", guardSelectStoryPart, SelectStoryPart.lens)
      .union("writeStory", guardWriteStoryPart, WriteStoryPart.lens)),

  toTitle: (page: WritePage) => "Write",

  path: /^\/write/,

  toUrl: (homePage: WritePage): string => "/write",

  ofUrl: (pathname: string): WritePage => WritePage.instance
}

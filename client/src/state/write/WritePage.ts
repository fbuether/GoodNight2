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


function assertNever(param: never): never {
  throw new Error(`Invalid Page kind in state WritePage: "${param}"`);
}

export const WritePage = {
  instance: {
    kind: "WritePage" as const,
    part: SelectStoryPart.instance
  },

  lens: <T>(id: P.Prism<T, WritePage>) => id
    .path("part", lens => lens
      .union("selectStory", guardSelectStoryPart, SelectStoryPart.lens)
      .union("writeStory", guardWriteStoryPart, WriteStoryPart.lens)),

  toTitle: (page: WritePage) => "Write",

  path: /^\/write/,

  toUrl: (page: WritePage): string => {
    switch (page.part.kind) {
      case "SelectStoryPart": return "/write";
      case "WriteStoryPart": return "/write/" + WriteStoryPart.toUrl(page.part);
      default: return assertNever(page.part);
    }
  },

  ofUrl: (pathname: string): WritePage => WritePage.instance
}

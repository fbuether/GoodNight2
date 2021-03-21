import * as P from "../ProtoLens";

import {OfUrl} from "../../util/UrlMapper";

import {SelectStoryPart} from "./SelectStoryPart";
import {CreateStoryPart} from "./CreateStoryPart";
import {WriteStoryPart} from "./WriteStoryPart";


export type WritePagePart =
    | SelectStoryPart
    | CreateStoryPart
    | WriteStoryPart;

export interface WritePage {
  kind: "WritePage";
  part: WritePagePart;
}


let guardSelectStoryPart = (a: WritePagePart): a is SelectStoryPart =>
    (a.kind == "SelectStoryPart");
let guardCreateStoryPart = (a: WritePagePart): a is CreateStoryPart =>
    (a.kind == "CreateStoryPart");
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
      .union("createStory", guardCreateStoryPart, CreateStoryPart.lens)
      .union("writeStory", guardWriteStoryPart, WriteStoryPart.lens)),

  toTitle: (page: WritePage) => "Write",

  path: /^\/write(.+)?$/,

  toUrl: (page: WritePage): string => {
    switch (page.part.kind) {
      case "SelectStoryPart": return "/write";
      case "CreateStoryPart": return "/write/create";
      case "WriteStoryPart": return "/write" + WriteStoryPart.toUrl(page.part);
      default: return assertNever(page.part);
    }
  },

  ofUrl: (pathname: string, matches: Array<string>): WritePage => {
    let subPathname = matches[1];

    return {
      kind: "WritePage" as const,
      part: OfUrl.union(subPathname,
        [SelectStoryPart, CreateStoryPart, WriteStoryPart],
        SelectStoryPart.instance)
    };
  }
}

import * as P from "../ProtoLens";

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

  lens: <T>(id: P.Prism<T, WritePage>) => id,

  toTitle: (page: WritePage) => "Write",

  path: /^\/write/,

  toUrl: (homePage: WritePage): string => "/write",

  ofUrl: (pathname: string): WritePage => WritePage.instance
}

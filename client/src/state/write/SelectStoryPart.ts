import * as P from "../ProtoLens";

import {StoryHeader} from "../../model/write/Story";


export interface SelectStoryPart {
  kind: "SelectStoryPart";
  stories: Array<StoryHeader> | null;
}


export const SelectStoryPart = {
  instance: {
    kind: "SelectStoryPart" as const,
    stories: null
  },

  path: /^\/?$/,

  lens: <T>(id: P.Prism<T, SelectStoryPart>) => id
    .prop("stories"),

  ofUrl: (pathName: string): SelectStoryPart => SelectStoryPart.instance
}

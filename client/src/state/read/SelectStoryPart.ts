import * as P from "../ProtoLens";

import {Story} from "../../model/read/Story";


export interface SelectStoryPart {
  kind: "SelectStoryPart";
  stories: Array<Story> | null;
}


export const SelectStoryPart = {
  instance: {
    kind: "SelectStoryPart" as const,
    stories: null
  },

  lens: <T>(id: P.Prism<T, SelectStoryPart>) => id
    .prop("stories")
}

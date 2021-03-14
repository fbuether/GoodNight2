import * as P from "../ProtoLens";

import {Story} from "../../model/write/Story";


export interface CreateStoryPart {
  kind: "CreateStoryPart";
}


export const CreateStoryPart = {
  instance: {
    kind: "CreateStoryPart" as const
  },

  lens: <T>(id: P.Prism<T, CreateStoryPart>) => id,

  path: /^\/create\/?$/,

  ofUrl: (pathName: string): CreateStoryPart => CreateStoryPart.instance
}

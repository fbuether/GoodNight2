import * as P from "../ProtoLens";

import {Story} from "../../model/write/Story";


export interface CreateStoryPart {
  kind: "CreateStoryPart";
  name: string;
}


export const CreateStoryPart = {
  instance: {
    kind: "CreateStoryPart" as const,
    name: ""
  },

  lens: <T>(id: P.Prism<T, CreateStoryPart>) => id
    .prop("name"),

  path: /^\/create\/?$/,

  ofUrl: (pathName: string): CreateStoryPart => CreateStoryPart.instance
}

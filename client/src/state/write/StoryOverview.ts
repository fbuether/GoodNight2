import * as P from "../ProtoLens";

import {Scene} from "../../model/write/Scene";


export interface StoryOverview {
  kind: "StoryOverview";
  story: string;
  scenes: Array<Scene> | null;
}


export const StoryOverview = {
  instance: (story: string) => ({
    kind: "StoryOverview" as const,
    story: story,
    scenes: null
  }),

  path: /^\/?$/,

  ofUrl: (pathname: string, matches: Array<string>, story: string)
  : StoryOverview =>
      StoryOverview.instance(story),

  lens: <T>(id: P.Prism<T, StoryOverview>) => id
    .prop("story")
    .prop("scenes")
}

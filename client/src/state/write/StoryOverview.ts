import * as P from "../ProtoLens";

import type {Scene} from "../../model/write/Scene";
import type {Category} from "../../model/write/Story";


export interface StoryOverview {
  kind: "StoryOverview";
  story: string;
  categories: Category | null;
}


export const StoryOverview = {
  instance: (story: string) => ({
    kind: "StoryOverview" as const,
    story: story,
    categories: null
  }),

  path: /^\/?$/,

  ofUrl: (pathname: string, matches: Array<string>, story: string)
  : StoryOverview =>
      StoryOverview.instance(story),

  lens: <T>(id: P.Prism<T, StoryOverview>) => id
    .prop("story")
    .prop("categories")
}

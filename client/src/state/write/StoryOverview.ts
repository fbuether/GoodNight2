import * as P from "../ProtoLens";


export interface StoryOverview {
  kind: "StoryOverview";
}


export const StoryOverview = {
  instance: {
    kind: "StoryOverview" as const
  },

  path: /^$/,

  ofUrl: (pathname: string): StoryOverview => StoryOverview.instance,

  lens: <T>(id: P.Prism<T, StoryOverview>) => id
}

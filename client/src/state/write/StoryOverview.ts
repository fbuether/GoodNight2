import * as P from "../ProtoLens";


export interface StoryOverview {
  kind: "StoryOverview"
}


export const StoryOverview = {
  instance: {
    kind: "StoryOverview" as const
  },

  lens: <T>(id: P.Prism<T, StoryOverview>) => id
}

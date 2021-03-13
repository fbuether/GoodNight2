import * as P from "../ProtoLens";


export interface StoryOverview {
  kind: "StoryOverview"
}


export const StoryOverview = {
  lens: <T>(id: P.Prism<T, StoryOverview>) => id
}

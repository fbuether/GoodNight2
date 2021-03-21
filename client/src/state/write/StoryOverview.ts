import * as P from "../ProtoLens";


export interface StoryOverview {
  kind: "StoryOverview";
  story: string;
}


export const StoryOverview = {
  instance: (story: string) => ({
    kind: "StoryOverview" as const,
    story: story
  }),

  path: /^\/?$/,

  ofUrl: (pathname: string, matches: Array<string>, story: string)
  : StoryOverview =>
      StoryOverview.instance(story),

  lens: <T>(id: P.Prism<T, StoryOverview>) => id
}

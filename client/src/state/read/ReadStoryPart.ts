import * as P from "../ProtoLens";

import type {Story} from "../../model/read/Story";

export interface ReadStoryPart {
  kind: "ReadStoryPart";
  story: Story | string;
}


export const ReadStoryPart = {
  instance: (story: string) => ({
    kind: "ReadStoryPart" as const,
    story: story,
  }),

  lens: <T>(id: P.Prism<T, ReadStoryPart>) => id
    .prop("story"),

  path: /^\/story\/(.+)$/,

  toUrl: (page: ReadStoryPart): string => {
    return typeof page.story == "string"
        ? "/story/" + page.story
        : "/story/" + page.story.urlname;
  },

  ofUrl: (pathname: string, matches: Array<string>): ReadStoryPart =>
      ReadStoryPart.instance(matches[1])
}

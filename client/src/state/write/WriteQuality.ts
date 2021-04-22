import * as P from "../ProtoLens";

import type {Quality} from "../../model/write/Quality";


export interface WriteQuality {
  kind: "WriteQuality";
  story: string; // urlname of story
  quality: Quality | null; // null if not loaded.
  raw: string; // "" if not loaded or new
  urlname: string | null; // null: new.

  isSaving: boolean;
  error: string | null;
}

export const WriteQuality = {
  instance: (story: string) => ({
    kind: "WriteQuality" as const,
    story: story,
    quality: null,
    raw: "",
    urlname: null,
    isSaving: false,
    error: null
  }),

  path: /^\/(quality\/(.+)|new-quality)$/,

  toUrl: (page: WriteQuality): string =>
      page.urlname == null
      ? "/new-quality"
      : "/quality/" + page.urlname,

  ofUrl: (pathname: string, matches: Array<string>, story: string)
  : WriteQuality => {
    if (matches[2] !== undefined) {
      return {
        ...WriteQuality.instance(story),
        urlname: matches[2]
      };
    }
    else {
      return WriteQuality.instance(story);
    }
  },

  lens: <T>(id: P.Prism<T, WriteQuality>) => id
    .prop("story")
    .prop("quality")
    .prop("raw")
    .prop("urlname")
    .prop("isSaving")
    .prop("error")
}

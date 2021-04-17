import * as P from "../ProtoLens";

import type {Scene} from "../../model/write/Scene";


export interface WriteScene {
  kind: "WriteScene";
  story: string; // urlname of story.
  scene: Scene | null; // null if not loaded.
  raw: string; // "" if not loaded or new; stores updated text from textarea.
  urlname: string | null; // null means: new scene.

  isSaving: boolean;
}

export const WriteScene = {
  instance: (story: string) => ({
    kind: "WriteScene" as const,
    story: story,
    scene: null,
    raw: "",
    urlname: null,
    isSaving: false
  }),

  path: /^\/(scene\/(.+)|new-scene)$/,

  toUrl: (page: WriteScene): string =>
      page.urlname == null
      ? "/new-scene"
      : "/scene/" + page.urlname,

  ofUrl: (pathname: string, matches: Array<string>, story: string)
  : WriteScene => {
    if (matches[2] !== undefined) {
      return {
        ...WriteScene.instance(story),
        urlname: matches[2]
      };
    }
    else {
      return WriteScene.instance(story);
    }
  },

  lens: <T>(id: P.Prism<T, WriteScene>) => id
    .prop("story")
    .prop("scene")
    .prop("raw")
    .prop("urlname")
    .prop("isSaving")
}

import * as P from "../ProtoLens";


export interface WriteScene {
  kind: "WriteScene";
  story: string; // urlname of story.
  scene: string | null; // null if not loaded.
  urlname: string | null; // null means: new scene.
}

export const WriteScene = {
  instance: (story: string) => ({
    kind: "WriteScene" as const,
    story: story,
    scene: "",
    urlname: null
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
        kind: "WriteScene" as const,
        story: story,
        scene: "",
        urlname: matches[2]
      };
    }
    else {
      return WriteScene.instance(story);
    }
  },

  lens: <T>(id: P.Prism<T, WriteScene>) => id
    .prop("scene")
}

import * as P from "../ProtoLens";


export interface WriteScene {
  kind: "WriteScene";
  scene: string;
  urlname: string | null; // null means: new scene.
}

export const WriteScene = {
  instance: {
    kind: "WriteScene" as const,
    scene: "",
    urlname: null
  },

  path: /^\/(scene\/(.+)|new-scene)$/,

  toUrl: (page: WriteScene): string =>
      page.urlname == null
      ? "/new-scene"
      : "/scene/" + page.urlname,

  ofUrl: (pathname: string, matches: Array<string>): WriteScene => {
    return WriteScene.instance;
  },

  lens: <T>(id: P.Prism<T, WriteScene>) => id
    .prop("scene")
}

import * as P from "./ProtoLens";


export interface HomePage {
  kind: "HomePage";
}


export const HomePage = {
  instance: {
    kind: "HomePage" as const
  },

  lens: <T>(id: P.Prism<T, HomePage>) => id,

  path: /^\/(home)?$/,

  toUrl: (homePage: HomePage): string => "/",

  ofUrl: (pathname: string): HomePage => HomePage.instance
}

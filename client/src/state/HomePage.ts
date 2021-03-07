import * as O from "optics-ts";

import {Page} from "./Page.ts";


export interface HomePage {
  kind: "HomePage";
}


export const HomePage = {
  instance: {
    kind: "HomePage" as const
  },

  guard: (a: Page): a is HomePage => (a.kind == "HomePage"),

  path: /^\/(home)?$/,

  toUrl: (homePage: HomePage): string => "/",

  ofUrl: (pathname: string): HomePage => HomePage.instance
}

import type {Pages} from "../state/Pages";
import type {State} from "../state/State";
import type {Dispatch} from "./Dispatch";


export interface PageDescriptor {
  state: Pages;
  url: string;
  title: string;

  // lifecycle
  onLoad?: (dispatch: Dispatch, state: State) => Promise<void>;

  requiresAuth?: true;
}



export interface PageMapper {
  path: RegExp;
  ofUrl: (matches: ReadonlyArray<string>) => PageDescriptor;
}


let pageMappers: Array<PageMapper> = [];

type OfUrlArg =  PageDescriptor
    | ((matches: ReadonlyArray<string>) => PageDescriptor);

export function registerPageMapper(path: RegExp, ofUrl: OfUrlArg) {
  if (pageMappers.find(m => m.path == path)) {
    return;
  }

  let desc = typeof ofUrl == "function"
      ? ofUrl
      : (matches: ReadonlyArray<string>) => ofUrl;

  pageMappers.push({ path: path, ofUrl: desc });
}


export function getPageMappers(): ReadonlyArray<PageMapper> {
  return pageMappers;
}

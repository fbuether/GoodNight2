import {Adventure} from "./read/Adventure";


export interface StartPage {
  kind: "start";
}

const startPagePath = /^\/($|start)/;



export interface ReadPage {
  kind: "read";
  story: string; // urlname
  user: string;
  adventure?: Adventure; // may be null if not yet loaded.
}

const readPagePath = /^\/read\/(.+)/;



export type Page =
    | StartPage
    | ReadPage;


function assertNever(param: never): never {
  throw new Error(`"asHref" received invalid state: "${param}"`);
}

export function asHref(page: Page) {
  switch (page.kind) {
    case "read": return "/read/" + page.story;
    case "start": return "/start";
    default: return assertNever(page);
  }
}


export function ofHref(url: URL): Page {
  let start = url.pathname.match(startPagePath);
  if (start != null) {
    return {
      kind: "start" as const
    };
  }

  let read = url.pathname.match(readPagePath);
  if (read != null) {
    return {
      kind: "read" as const,
      story: read[1],
      user: "Mrs. Hollywookle"
    };
  }

  // default.
  return {
    kind: "start"
  };
}

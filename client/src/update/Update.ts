import type State from "../model/State";

import navigate from "./Navigate";
import type {Page} from "../model/Page";

import {showStory} from "./LoadStory";
import type {Adventure} from "../model/read/Adventure";

import {showOption} from "./DoOption";
import type {Consequence} from "../model/read/Consequence";


export type Update =
    | { kind: "Navigate", page: Page }
    | { kind: "ShowStory", adventure: Adventure }
    | { kind: "ReadOption", consequence: Consequence };


function assertNever(param: never): never {
  throw new Error(`"update" received invalid state: "${param}"`);
}

export default function update(state: State, update: Update): State {
  console.log("update state", update);
  switch (update.kind) {
    case "Navigate": return navigate(state, update.page);
    case "ShowStory": return showStory(state, update.adventure);
    case "ReadOption": return showOption(state, update.consequence);
    default: return assertNever(update);
  }
}

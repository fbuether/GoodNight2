import * as PreactHooks from "preact/hooks";

import {ReadPage as ReadPageState} from "../../state/read/ReadPage";

import SelectStory from "./SelectStory";

import Scene from "../play/Scene";
import State from "../play/State";
import Log from "../play/Log";


function assertNever(param: never): never {
  throw new Error(`Invalid ReadPage kind in ReadPage: "${param}"`);
}

export default function ReadPage(page: ReadPageState) {
  switch (page.part.kind) {
    case "SelectStoryPart": return <SelectStory {...page.part} />;
    case "ReadStoryPart": return <>"reading!"</>;
    default: assertNever(page.part);
  }
}

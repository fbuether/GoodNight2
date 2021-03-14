import * as PreactHooks from "preact/hooks";

import {WritePage as State} from "../../state/write/WritePage";

import CreateStory from "./CreateStory";
import SelectStory from "./SelectStory";


function assertNever(param: never): never {
  throw new Error(`Invalid WritePage kind in WritePage: "${param}"`);
}

export default function WritePage(page: State) {
  switch (page.part.kind) {
    case "SelectStoryPart": return <SelectStory {...page.part} />;
    case "CreateStoryPart": return <CreateStory {...page.part} />;
    case "WriteStoryPart": return <>ooooo, writea story</>;
    default: assertNever(page.part);
  }
}

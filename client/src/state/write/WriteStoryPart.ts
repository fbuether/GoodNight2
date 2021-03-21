import * as P from "../ProtoLens";

import {OfUrl} from "../../util/UrlMapper";

import {Story} from "../../model/write/Story";
// import {WritePagePart} from "./WritePage";

import {StoryOverview} from "./StoryOverview";
// import {SceneOverview} from "./SceneOverview";
// import {QualityOverview} from "./QualityOverview";
// import {TagOverview} from "./TagOverview";
// import {CategoryOverview} from "./CategoryOverview";
import {WriteScene} from "./WriteScene";
// import {EditQuality} from "./EditQuality";


export type WritePart =
    | StoryOverview
    // | SceneOverview
    // | QualityOverview
    // | TagOverview
    // | CategoryOverview
    | WriteScene
    // | EditQuality
;


export interface WriteStoryPart {
  kind: "WriteStoryPart";
  story: Story | string; // if not a story, then its urlname.
  part: WritePart;
}

function assertNever(param: never): never {
  throw new Error(`Invalid Page kind in state WriteStoryPart: "${param}"`);
}

let guardStoryOverview = (a: WritePart): a is StoryOverview =>
    (a.kind == "StoryOverview");
let guardWriteScene = (a: WritePart): a is WriteScene =>
    (a.kind == "WriteScene");

export const WriteStoryPart = {
  instance: {
    kind: "WriteStoryPart" as const,
    story: "",
    part: StoryOverview.instance
  },

  lens: <T>(id: P.Prism<T, WriteStoryPart>) => id
    .prop("story")
    .path("part", (lens: any) => lens
      .union("storyOverview", guardStoryOverview, StoryOverview.lens)
      .union("writeScene", guardWriteScene, WriteScene.lens)),

  path: /^\/story\/([^/]+)(.+)?$/,

  toUrl: (part: WriteStoryPart): string => {
    let prefix = typeof part.story == "string"
        ? "/story/" + part.story
        : "/story/" + part.story.urlname;

    switch (part.part.kind) {
      case "StoryOverview": return prefix;
      case "WriteScene": return prefix + WriteScene.toUrl(part.part);
      default: return assertNever(part.part);
    }
  },

  ofUrl: (pathname: string, matches: Array<string>): WriteStoryPart => {
    let storyUrlname = matches[1];
    let subPagePath = matches[2];

    console.log("writestorypart.ofurl", matches);

    return {
      kind: "WriteStoryPart" as const,
      story: storyUrlname,
      part: OfUrl.union(subPagePath, [WriteScene, StoryOverview],
         StoryOverview.instance)
    };
  }
}

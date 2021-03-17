import * as P from "../ProtoLens";

import {Story} from "../../model/write/Story";
// import {WritePagePart} from "./WritePage";

import {StoryOverview} from "./StoryOverview";
// import {SceneOverview} from "./SceneOverview";
// import {QualityOverview} from "./QualityOverview";
// import {TagOverview} from "./TagOverview";
// import {CategoryOverview} from "./CategoryOverview";
// import {EditScene} from "./EditScene";
// import {EditQuality} from "./EditQuality";


export type WritePart =
    | StoryOverview
    // | SceneOverview
    // | QualityOverview
    // | TagOverview
    // | CategoryOverview
    // | EditScene
    // | EditQuality
;


export interface WriteStoryPart {
  kind: "WriteStoryPart";
  story: Story | null;
  part: WritePart;
}


let guardStoryOverview = (a: WritePart): a is StoryOverview =>
    (a.kind == "StoryOverview");

export const WriteStoryPart = {
  instance: {
    kind: "WriteStoryPart" as const,
    story: null,
    part: StoryOverview.instance
  },

  lens: <T>(id: P.Prism<T, WriteStoryPart>) => id
    .prop("story")
    .path("part", (lens: any) => lens
          .union("storyOverview", guardStoryOverview, StoryOverview.lens)),

  path: /^\/story\/(.+)$/,

  toUrl: (part: WriteStoryPart): string => {
    return part.story ? "/story/" + part.story.urlname : "";
  },

  ofUrl: (pathname: string): WriteStoryPart => WriteStoryPart.instance
}
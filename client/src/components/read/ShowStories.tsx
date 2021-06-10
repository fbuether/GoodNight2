import * as Preact from "preact";

import type {PageDescriptor} from "../../core/PageDescriptor";

import type {Story} from "../../model/read/Story";
import type {Loadable} from "../../state/Loadable";

import LoadableLoader from "../../components/common/LoadableLoader";
import {ShowableStory} from "./StoryCard";
import StoryCard from "./StoryCard";


export interface ShowStories {
  stories: Loadable<Array<ShowableStory>>;
  page: (urlname: string) => PageDescriptor;
  children?: Preact.ComponentChildren;
  cols?: number;
}


export default function ShowStories(state: ShowStories) {
  var rows = "";
  if (state.cols == 2) {
    rows = "row-cols-md-2";
  }
  else if (state.cols && state.cols >=3) {
    rows = "row-cols-sm-2 row-cols-md-3";
  }

  return LoadableLoader(state.stories, stories => {
    return (
      <div class={`row cards row-cols-1 ${rows} g-3 mt-0`}>
        {state.children}
        {stories.map(story =>
          <StoryCard story={story} page={state.page} />)}
      </div>
    );
  });
}

import type {PageDescriptor} from "../../core/PageDescriptor";

import type {Story} from "../../model/read/Story";
import type {Loadable} from "../../state/Loadable";

import Error from "../../components/common/Error";
import LoadableLoader from "../../components/common/LoadableLoader";
import Loading from "../../components/common/Loading";

import {ShowableStory} from "./StoryCard";
import StoryCard from "./StoryCard";


export interface ShowStories {
  stories: Loadable<Array<ShowableStory>>;
  page: (urlname: string) => PageDescriptor;
}


export default function ShowStories(state: ShowStories) {
  return LoadableLoader(state.stories, stories => {
    return (
      <div class="row cards row-cols-1 row-cols-sm-2 row-cols-md-3 g-3 mt-0">
        {stories.map(story =>
          <StoryCard story={story} page={state.page} />)}
      </div>
    );
  });
}

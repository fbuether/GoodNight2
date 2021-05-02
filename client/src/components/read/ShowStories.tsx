import type {PageDescriptor} from "../../core/PageDescriptor";

import type {Story} from "../../model/read/Story";
import type {Loadable} from "../../state/Loadable";

import Error from "../../components/common/Error";
import Loading from "../../components/common/Loading";

import {ShowableStory} from "./StoryCard";
import StoryCard from "./StoryCard";


export interface ShowStories {
  stories: Loadable<Array<ShowableStory>>;
  page: (urlname: string) => PageDescriptor;
}


export default function ShowStories(state: ShowStories) {
  if (state.stories.state == "unloaded" || state.stories.state == "loading") {
    return <Loading />;
  }
  else if (state.stories.state == "failed") {
    return <Error message={state.stories.error} />
  }
  else {
    return (
      <div class="row cards row-cols-1 row-cols-sm-2 row-cols-md-3 g-3 mt-0">
        {state.stories.result.map(story =>
          <StoryCard story={story} page={state.page} />)}
      </div>
    );
  }
}

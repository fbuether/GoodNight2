import {Dispatch} from "../../core/Dispatch";
import type {Story} from "../../model/read/Story";
import type {Loadable} from "../../state/Loadable";

import {ReadStory} from "../../state/page/read/ReadStory";

import ShowStories from "../../components/read/ShowStories";


export interface StoryOverview {
  page: "StoryOverview";
  stories: Loadable<Array<Story>>;
}


export function StoryOverview(state: StoryOverview) {
  var page = (urlname: string) => ReadStory.page(urlname);

  return (
    <div id="centre" class="row px-0 g-0">
      <h1>Geschichten bei GoodNight</h1>
      <p>Wähle eine Geschichte aus, die du lesen möchtest.</p>
      <ShowStories stories={state.stories} page={page} />
    </div>
  );
}


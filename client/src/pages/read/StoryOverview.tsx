import type {Story} from "../../model/read/Story";
import type {Loadable} from "../../state/Loadable";

import ShowStories from "../../components/read/ShowStories";


export interface StoryOverview {
  page: "StoryOverview";
  stories: Loadable<Array<Story>>;
}


export function StoryOverview(state: StoryOverview) {
  return (
    <div id="centre" class="row px-0 g-0">
      <h1>Geschichten bei GoodNight</h1>
      <p>Wähle eine Geschichte aus, die du lesen möchtest.</p>
      <ShowStories {...state.stories} />
    </div>
  );
}


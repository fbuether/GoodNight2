import {Dispatch} from "../../core/Dispatch";
import type {Story} from "../../model/read/Story";
import type {Loadable} from "../../state/Loadable";

import {StoryOverview as State} from "../../state/page/read/StoryOverview";
import {ReadStory} from "../../state/page/read/ReadStory";

import ShowStories from "../../components/read/ShowStories";


export function StoryOverview(state: State) {
  var page = (urlname: string) => ReadStory.page(urlname);

  return (
    <div id="centre" class="row px-0 g-0">
      <h1>Geschichten bei GoodNight</h1>
      <div class="row">
        <div class="col-6 col-md-4">
          <p>Lese eine Geschichte weiter:</p>
          <ShowStories stories={state.myStories} page={page}
            onDelete={state.onDelete} />
        </div>
        <div class="col-6 col-md-8">
          <p>Oder wähle eine neue Geschichte, um sie zu beginnen:</p>
          <ShowStories stories={state.stories} page={page} cols={2} />
        </div>
      </div>
    </div>
  );
}


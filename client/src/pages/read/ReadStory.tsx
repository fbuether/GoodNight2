import type {Adventure} from "../../model/read/Adventure";
import type {Story} from "../../model/read/Story";
import type {Option} from "../../model/read/Action";
import type {ReadStory as State} from "../../state/page/read/ReadStory";

import LoadableLoader from "../../components/common/LoadableLoader";

import Log from "../../components/read/Log";
import Action from "../../components/read/Action";
import Player from "../../components/read/Player";


function ReadStoryLoaded(state: State, story: Story, adventure: Adventure) {
  let onOption = (option: string) => state.onOption(state, option);

  return (
    <div id="centre" class="row px-0 g-0">
      <div id="text" class="col-sm-8">
        <h1 id="banner">{story.name}</h1>
        <Log entries={adventure.history}></Log>
        <Action action={adventure.current} onOption={onOption} />
      </div>
      <div id="side" class="col-sm-4">
        <hr class="w-75 mx-auto mt-4 mb-5" />
        <Player {...adventure.player} />
      </div>
    </div>
  );
}

export function ReadStory(state: State) {
  return LoadableLoader(state.adventure, adventure =>
      LoadableLoader(state.story, story =>
          ReadStoryLoaded(state, story, adventure)));
}

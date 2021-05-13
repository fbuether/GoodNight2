import type {Adventure} from "../../model/read/Adventure";
import type {Loadable} from "../../state/Loadable";

import type {ReadStory as State} from "../../state/page/read/ReadStory";

import Error from "../../components/common/Error";
import Loading from "../../components/common/Loading";
import LoadableLoader from "../../components/common/LoadableLoader";


function ReadStoryLoaded(state: State, adventure: Adventure) {
  return (
    <div id="centre" class="row px-0 g-0">
      <div id="text" class="col-sm-8">
        <h1>{adventure.story.name}</h1>

        <p>{adventure.story.description}</p>
      </div>
    </div>
  );
      //   <h1 id="banner">{adventure.story.name}</h1>
      //   <Log entries={adventure.history}></Log>
      //   <Scene {...adventure.current}></Scene>
      // </div>
      // <div id="side" class="col-sm-4">
      //   <hr class="w-75 mx-auto mt-4 mb-5" />
      //   <State {...adventure.player}></State>

}

export function ReadStory(state: State) {
  return LoadableLoader(state.adventure, adventure =>
      ReadStoryLoaded(state, adventure));
}

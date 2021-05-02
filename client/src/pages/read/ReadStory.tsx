import type {Story} from "../../model/read/Story";
import type {Loadable} from "../../state/Loadable";

import Error from "../../components/common/Error";
import Loading from "../../components/common/Loading";


export interface ReadStory {
  page: "ReadStory";
  urlname: string;
  story: Loadable<Story>;
}

export function ReadStory(state: ReadStory) {
  let story = state.story;

  switch (story.state) {
    case "unloaded":
    case "loading":
      return <Loading />;
    case "failed":
      return <Error message={story.error} />;
    case "loaded":
      let res = story.result;
      return (
        <div id="centre" class="row px-0 g-0">
          <div id="text" class="col-sm-8">
            <h1>{res.name}</h1>

            <p>{res.description}</p>
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
}

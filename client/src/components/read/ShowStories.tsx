import type {Story} from "../../model/read/Story";
import type {Loadable} from "../../state/Loadable";

import Error from "../../components/common/Error";
import Loading from "../../components/common/Loading";
import StoryCard from "./StoryCard";


export default function ShowStories(state: Loadable<Array<Story>>) {
  if (state.state == "unloaded" || state.state == "loading") {
    return <Loading />;
  }
  else if (state.state == "failed") {
    return <Error message={state.error} />
  }
  else {
    return (
      <div class="row cards row-cols-1 row-cols-sm-2 row-cols-md-3 g-3 mt-0">
        {state.result.map(story => <StoryCard story={story} />)}
      </div>
    );
  }
}

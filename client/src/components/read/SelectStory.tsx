import * as PreactHooks from "preact/hooks";
import DispatchContext from "../../DispatchContext";
import useAsyncEffect from "../../ui/useAsyncEffect";
import request from "../../Request";

import {Story} from "../../model/read/Story";

import {State, Dispatch} from "../../state/State";
import {SelectStoryPart as PartState} from "../../state/read/SelectStoryPart";

import Loading from "../common/Loading";
import Link from "../common/Link";


function loadStories(dispatch: Dispatch) {
  return async () => {
    let stories = await request<Array<Story>>("GET", "/api/v1/read/stories");
    if (stories.isError) {
      return;
    }

    dispatch(State.lens.page.read.part.selectStory.stories
      .set(stories.message));
  }
}

function StoryCard(state: { story: Story }) {
  let link = State.lens.page.read.part.set({
    kind: "ReadStoryPart" as const,
    story: state.story
  });

  return (
    <div class="col">
      <div class="card">
        <div class="card-body">
          <h5>
            <Link class="stretched-link" target={link}>
              {state.story.name}
            </Link>
          </h5>
          <p>{state.story.description}</p>
        </div>
      </div>
    </div>
  );
}


function ShowStories(state: { stories: Array<Story> | null }) {
  const dispatch = PreactHooks.useContext(DispatchContext);

  if (state.stories == null) {
    useAsyncEffect(loadStories(dispatch));
    return <Loading />;
  }
  else {
    return (
      <>
      <div>weee, {state.stories.length} stories!</div>
      <div class="row cards row-cols-1 row-cols-sm-2 row-cols-md-3 g-3 mt-0">
        {state.stories.map(story => <StoryCard story={story} />)}
      </div>
      </>
    );
  }
}



export default function SelectStory(part: PartState) {
  return (
    <div id="centre" class="row px-0 g-0">
      <h1>Geschichten bei GoodNight</h1>
      <p>Wähle eine Geschichte aus, die du lesen möchtest.</p>
      <ShowStories stories={part.stories} />
    </div>
  );
}

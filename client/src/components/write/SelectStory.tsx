import * as PreactHooks from "preact/hooks";
import DispatchContext from "../../DispatchContext";
import useAsyncEffect from "../../ui/useAsyncEffect";
import request from "../../Request";

import {Story} from "../../model/write/Story";
import {State, Dispatch} from "../../state/State";
import {SelectStoryPart} from "../../state/write/SelectStoryPart";
import {CreateStoryPart} from "../../state/write/CreateStoryPart";
import {StoryOverview} from "../../state/write/StoryOverview";

import Link from "../common/Link";
import Icon from "../common/Icon";
import Loading from "../common/Loading";


function loadStories(dispatch: Dispatch, state: SelectStoryPart) {
  return async () => {
    let stories = await request<Array<Story>>(
      "GET", "api/v1/write/stories");
    if (stories.isError) {
      return;
    }

    dispatch(State.lens.page.write.part.selectStory.stories
      .set(stories.message));
  }
}


export default function SelectStory(state: SelectStoryPart) {
  const dispatch = PreactHooks.useContext(DispatchContext);

  let inner;
  if (state.stories == null) {
    useAsyncEffect(loadStories(dispatch, state));

    inner = <Loading />;
  }
  else {
    let stories = state.stories.map(story => {
      let link = State.lens.page.write.part.set({
        kind: "WriteStoryPart" as const,
        story: story.urlname,
        part: StoryOverview.instance(story.urlname)
      });

      return (
        <div class="col">
          <div class="card">
            <div class="card-body">
              <h5>
                <Link class="stretched-link" target={link}>
                  {story.name}
                </Link>
              </h5>
              <p>{story.description}</p>
            </div>
          </div>
        </div>
      );
    });

    let newLink = State.lens.page.write.part.set(CreateStoryPart.instance);

    inner = (
      <div class="row cards row-cols-1 row-cols-sm-2 row-cols-md-3 g-3 mt-0">
        <div class="col">
          <div class="card new">
            <div class="card-body">
              <h5>
                <Icon name="sundial" class="top mr-2" />
                <Link class="stretched-link" target={newLink}>
                  Neue Geschichte…
                </Link>
              </h5>
              <p>Beginne eine neue Geschichte, ein neues Abenteuer.</p>
            </div>
          </div>
        </div>

        {stories}
      </div>
    );
  }

  return (
    <div id="centre" class="row px-0 g-0">
      <h1>Schreibe eine Geschichte</h1>
      <p>
        Wähle eine Geschichte, die du schreiben willst, oder erstelle eine
        neue Geschichte.
      </p>
      {inner}
    </div>
  );
}

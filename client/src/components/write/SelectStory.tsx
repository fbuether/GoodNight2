import * as PreactHooks from "preact/hooks";
import DispatchContext from "../../DispatchContext";
import useAsyncEffect from "../../ui/useAsyncEffect";

import {Story} from "../../model/write/Story";
import {State, Dispatch} from "../../state/State";
import {SelectStoryPart} from "../../state/write/SelectStoryPart";
import {StoryOverview} from "../../state/write/StoryOverview";

import Link from "../common/Link";
import Loading from "../common/Loading";


function loadStories(dispatch: Dispatch, state: SelectStoryPart) {
  return async () => {

    let stories: Array<Story> | null = [
      {name:"abc",urlname:"abc"},
      {name:"abc2",urlname:"abc2"}
    ];

    // await new Promise( resolve => setTimeout(resolve, 500) );

    dispatch(State.lens.page.write.part.selectStory.stories.set(stories));
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
        story: story,
        part: StoryOverview.instance
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
              <p>({story.urlname})</p>
            </div>
          </div>
        </div>
      );
    });

    inner = (
      <div class="row row-cols-md-3">{stories}</div>
    );
  }

  return (
    <div id="centre" class="row px-0 g-0">
      <h1>Schreibe eine Geschichte</h1>
      <p>Wähle eine Geschichte, an der du schreiben willst, oder erstelle eine neue Geschichte.</p>
      {inner}
    </div>
  );
}

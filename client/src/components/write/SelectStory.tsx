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
      {name:"Hels Schlucht",urlname:"hels-schlucht",
       description: "In der Tiefe warten die letzten Menschen auf einen Helden, der den Toten wieder Ruhe bringt."},
      {name:"abc2",urlname:"abc2",description: "okay, this is"},
      {name:"abc",urlname:"abc",description: "okay, this is"},
      {name:"abc2",urlname:"abc2",description: "okay, this is"},
      {name:"abc",urlname:"abc",description: "okay, this is"},
      {name:"abc2",urlname:"abc2",description: "okay, this is"},
      {name:"abc",urlname:"abc",description: "okay, this is"},
      {name:"abc2",urlname:"abc2",description: "okay, this is"},
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
              <p>{story.description}</p>
            </div>
          </div>
        </div>
      );
    });

    inner = (
      <div class="row cards row-cols-1 row-cols-sm-2 row-cols-md-3 g-3 mt-0">
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

import * as PreactHooks from "preact/hooks";
import DispatchContext from "../../DispatchContext";
import useAsyncEffect from "../../ui/useAsyncEffect";
import request from "../../Request";

import {State, Dispatch} from "../../state/State";
import {Story} from "../../model/write/Story";

import {SelectStoryPart} from "../../state/write/SelectStoryPart";
import {WriteStoryPart} from "../../state/write/WriteStoryPart";
import {StoryOverview} from "../../state/write/StoryOverview";

import Link from "../common/Link";
import Icon from "../common/Icon";
import Loading from "../common/Loading";


function loadStory(dispatch: Dispatch, state: WriteStoryPart, name: string) {
  return async () => {
    let response = await request<Story>("GET", `api/v1/write/stories/${name}`);
    if (response.isError) {
      return;
    }

    dispatch(State.lens.page.write.part.writeStory.story.set(response.message));
  };
}


export default function WriteStory(state: WriteStoryPart) {
  const dispatch = PreactHooks.useContext(DispatchContext);

  if (typeof state.story == "string") {
    useAsyncEffect(loadStory(dispatch, state, state.story));
    return (
      <div id="centre">
        <Loading class="mt-4" />
      </div>
    );
  }

  let toBase = State.lens.page.write.part.writeStory.part
    .set(StoryOverview.instance);

  return (
    <div id="centre" class="px-0">
      <h1><Link target={toBase}>Schreibe: {state.story.name}</Link></h1>
      <div class="row">
        <div class="col-3">
          Szenen
        </div>
        <div class="col-3">
          Qualitäten
        </div>
        <div class="col-3">
          Kategorien
        </div>
        <div class="col-3">
          Tags
        </div>
      </div>


    </div>
  );
}

import * as PreactHooks from "preact/hooks";
import DispatchContext from "../../DispatchContext";
import useAsyncEffect from "../../ui/useAsyncEffect";
import request from "../../Request";

import {State, Dispatch} from "../../state/State";
import {Story} from "../../model/write/Story";

import {SelectStoryPart} from "../../state/write/SelectStoryPart";
import {WriteStoryPart} from "../../state/write/WriteStoryPart";

import Link from "../common/Link";
import Icon from "../common/Icon";
import Loading from "../common/Loading";


function loadStory(dispatch: Dispatch, state: WriteStoryPart, name: string) {
  return async () => {
    let response = await request<Story>("GET", `api/v1/write/story/${name}`);
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
        <Loading />
      </div>
    );
  }

  return (
    <div id="centre" class="row px-0 g-0">
      <h1>Schreibe: {state.story.name}</h1>
      <p>
        Eine neue Geschichte beginnt stets mit einem Namen. Dieser Name kann
        später geändert werden, allerdings nicht seine URL-Form.
      </p>
    </div>
  );
}

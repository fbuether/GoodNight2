import {JSX} from "preact";
import request from "../../Request";
import * as PreactHooks from "preact/hooks";

import DispatchContext from "../../DispatchContext";
import useAsyncEffect from "../../ui/useAsyncEffect";

import {Story} from "../../model/write/Story";
import {State, Dispatch} from "../../state/State";
import {CreateStoryPart} from "../../state/write/CreateStoryPart";
import {WriteStoryPart} from "../../state/write/WriteStoryPart";

import Link from "../common/Link";
import Loading from "../common/Loading";




function submit(dispatch: Dispatch, state: CreateStoryPart) {
  return async(event: JSX.TargetedEvent<HTMLFormElement, Event>) => {
    event.preventDefault();

    let response = await request<Story>("POST", "api/v1/write/stories", {
      name: state.name
    });

    if (response.isError) {
      return;
    }

    dispatch(State.lens.page.write.part.set({
      ...WriteStoryPart.instance,
      story: response.message
    }));
  };
}


function setNameOfStory(dispatch: Dispatch) {
  return (event: JSX.TargetedEvent<HTMLInputElement, Event>) => {
    dispatch(State.lens.page.write.part.createStory.name
      .set(event.currentTarget.value));
  };
}



export default function CreateStory(state: CreateStoryPart) {
  const dispatch = PreactHooks.useContext(DispatchContext);

  return (
    <div id="centre" class="row px-0 g-0">
      <h1>Eine neue Geschichte</h1>
      <p>
        Eine neue Geschichte beginnt stets mit einem Namen. Dieser Name kann
        später geändert werden, allerdings nicht seine URL-Form.
      </p>
      <form class="my-3 w-75 mx-auto" onSubmit={submit(dispatch, state)}>
        <label for="nameOfStory" class="form-label">
          Der Name der neuen Geschichte
        </label>
        <input id="nameOfStory" type="text" class="form-control"
          onChange={setNameOfStory(dispatch)} />
        <div class="buttons w-75 mx-auto mt-3 text-end">
          <button type="submit" class="btn btn-primary">
            Erstellen
          </button>
        </div>
      </form>
    </div>
  );
}

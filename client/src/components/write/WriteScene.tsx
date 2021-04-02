import {JSX} from "preact";
import request from "../../Request";
import * as PreactHooks from "preact/hooks";

import DispatchContext from "../../DispatchContext";
import useAsyncEffect from "../../ui/useAsyncEffect";

import {Scene} from "../../model/write/Scene";

import {State, Dispatch} from "../../state/State";
import {WriteScene as WriteSceneState} from "../../state/write/WriteScene";

import Loading from "../common/Loading";
import SceneHelp from "./SceneHelp";


function submit(dispatch: Dispatch, state: WriteSceneState) {
  return async(event: JSX.TargetedEvent<HTMLFormElement, Event>) => {
    event.preventDefault();

    let param = {
      text: state.scene
    };

    console.log("state:", state);

    let response = state.urlname === null
        ? await request<Scene>("POST", `/api/v1/write/story/${state.story}/scenes`, param)
        : await request<Scene>("PUT", `/api/v1/write/story/${state.story}/scenes/${state.urlname}`, param);

    if (response.isError) {
      return;
    }

    dispatch(State.lens.page.write.part.writeStory.part.writeScene.set({
      kind: "WriteScene" as const,
      scene: response.message.raw,
      story: state.story,
      urlname: response.message.urlname
    }));
  };
}



function setText(dispatch: Dispatch) {
  return (event: JSX.TargetedEvent<HTMLTextAreaElement, Event>) => {
    dispatch(State.lens.page.write.part.writeStory.part.writeScene.scene
      .set(event.currentTarget.value));
  };
}



export default function WriteScene(state: WriteSceneState) {
  const dispatch = PreactHooks.useContext(DispatchContext);

  let title = state.urlname === null ? "Neue Szene" : "Szene bearbeiten";

  console.log(state);

  if (state.urlname !== null && state.scene === null) {
    // todo: request this single scene, store its raw text in state.scene.

    return <Loading />;
  }


  return (
    <div class="row">

      <form class="col-8" onSubmit={submit(dispatch, state)}>
        <h2>{title}</h2>

        <textarea class="form-control"
          onChange={setText(dispatch)}>{state.scene}
        </textarea>

        <div class="buttons w-75 mx-auto mt-3 text-end">
          <button type="submit" class="btn btn-primary">
            Speichern
          </button>
        </div>

      </form>

      <div class="col-4">
        <SceneHelp />
      </div>
    </div>
  );
}

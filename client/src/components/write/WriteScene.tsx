import {JSX} from "preact";
import request from "../../Request";
import * as PreactHooks from "preact/hooks";

import DispatchContext from "../../DispatchContext";
import useAsyncEffect from "../../ui/useAsyncEffect";

import {Scene} from "../../model/write/Scene";

import {State, Dispatch} from "../../state/State";
import {WriteScene as WriteSceneState} from "../../state/write/WriteScene";

import Loading from "../common/Loading";
import Icon from "../common/Icon";
import ScalingTextarea from "../common/ScalingTextarea";
import SceneHelp from "./SceneHelp";


function submit(dispatch: Dispatch, state: WriteSceneState) {
  return async(event: JSX.TargetedEvent<HTMLFormElement, Event>) => {
    event.preventDefault();

    let param = { text: state.raw };
    let response = state.urlname === null
        ? await request<Scene>("POST",
            `/api/v1/write/story/${state.story}/scenes`, param)
        : await request<Scene>("PUT",
            `/api/v1/write/story/${state.story}/scenes/${state.urlname}`,
            param);

    if (response.isError) {
      return;
    }

    dispatch(State.lens.page.write.part.writeStory.part.writeScene.set({
      kind: "WriteScene" as const,
      scene: response.message,
      story: state.story,
      raw: response.message.raw,
      urlname: response.message.urlname
    }));
  };
}

function loadScene(dispatch: Dispatch, state: WriteSceneState) {
  return async () => {
    let sceneResponse = await request<Scene>("GET",
      `api/v1/write/story/${state.story}/scenes/${state.urlname}`);

    if (sceneResponse.isError) {
      return;
    }

    let scene = sceneResponse.message;

    dispatch(state => {
      let intermediate = State.lens.page.write.part.writeStory.part
        .writeScene.scene
        .set(scene)(state);
      return State.lens.page.write.part.writeStory.part.writeScene.raw
        .set(scene.raw)(intermediate);
    });
  };
}


function setText(dispatch: Dispatch) {
  return (event: JSX.TargetedEvent<HTMLTextAreaElement, Event>) => {
    dispatch(State.lens.page.write.part.writeStory.part.writeScene.raw
      .set(event.currentTarget.value));
  };
}



export default function WriteScene(state: WriteSceneState) {
  const dispatch = PreactHooks.useContext(DispatchContext);

  let title = state.urlname === null
      ? "Neue Szene"
      : "Szene bearbeiten";

  if (state.urlname !== null && state.scene === null) {
    useAsyncEffect(loadScene(dispatch, state));
    return <Loading />;
  }

  let text = state.scene != null ? state.scene.raw : state.raw;

  return (
    <div class="row">
      <form class="col-8" onSubmit={submit(dispatch, state)}>
        <h2>{title}</h2>

        <ScalingTextarea class="form-control larger"
          onChange={setText(dispatch)}
          content={text} />

        <div class="buttons w-75 mx-auto mt-3 text-end">
          <button type="submit" class="btn btn-primary">
            <Icon name="save" class="mr-2" />
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

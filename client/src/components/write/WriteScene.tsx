import {JSX} from "preact";
import request from "../../Request";
import * as PreactHooks from "preact/hooks";

import DispatchContext from "../../DispatchContext";
import useAsyncEffect from "../../ui/useAsyncEffect";
import {noEarlierThan} from "../../util/Timer";

import {Scene} from "../../model/write/Scene";

import {State, Dispatch} from "../../state/State";
import {WriteScene as WriteSceneState} from "../../state/write/WriteScene";
import {WriteStoryPart} from "../../state/write/WriteStoryPart";

import Loading from "../common/Loading";
import Icon from "../common/Icon";
import Link from "../common/Link";
import ScalingTextarea from "../common/ScalingTextarea";
import SceneHelp from "./SceneHelp";


function submit(dispatch: Dispatch, state: WriteSceneState) {
  return async(event: JSX.TargetedEvent<HTMLFormElement, Event>) => {
    event.preventDefault();

    dispatch(State.lens.page.write.part.writeStory.part.writeScene.isSaving
      .set(true));

    let param = { text: state.raw };
    let req = state.urlname === null
        ? request<Scene>("POST",
            `/api/v1/write/stories/${state.story}/scenes`, param)
        : request<Scene>("PUT",
            `/api/v1/write/stories/${state.story}/scenes/${state.urlname}`,
            param);

    let response = await noEarlierThan(200, req);

    let newState;
    switch (response.kind) {
      case "result":
        newState = {
          ...WriteSceneState.instance(state.story),
          scene: response.message,
          raw: response.message.raw,
          urlname: response.message.urlname
        };
        break;
      case "error":
        newState = {
          ...state,
          isSaving: false,
          error: response.message
        };
        break;
    }

    dispatch(State.lens.page.write.part.writeStory.part.writeScene
      .set(newState));
  };
}

function loadScene(dispatch: Dispatch, state: WriteSceneState) {
  return async () => {
    let sceneResponse = await request<Scene>("GET",
      `api/v1/write/stories/${state.story}/scenes/${state.urlname}`);

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

  let saveButton = state.isSaving
      ? <div type="submit" class="btn btn-primary disabled loading">
          <Icon name="empty-hourglass" class="mr-2" />
          Speichere…
        </div>
      : <button type="submit" class="btn btn-primary">
          <Icon name="save" class="mr-2" />
          Speichern
        </button>;

  let error = state.error !== null
      ? <div class="alert alert-danger alert-raw my-3">{state.error}</div>
      : <></>;

  let returnLink = State.lens.page.write.part.set(
    WriteStoryPart.instance(state.story));

  return (
    <div class="row">
      <form class="col-8" onSubmit={submit(dispatch, state)}>
        <h2>{title}</h2>

        <ScalingTextarea class="form-control larger"
          onChange={setText(dispatch)}
          content={text} />

        {error}
        <div class="d-flex w-75 mx-auto mt-3 justify-content-between align-items-center">
          <Link target={returnLink}>Zurück</Link>
          {saveButton}
        </div>
      </form>
      <div class="col-4">
        <SceneHelp />
      </div>
    </div>
  );
}

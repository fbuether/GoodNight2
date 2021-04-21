import {JSX} from "preact";
import request from "../../Request";
import * as PreactHooks from "preact/hooks";

import DispatchContext from "../../DispatchContext";
import useAsyncEffect from "../../ui/useAsyncEffect";
import {noEarlierThan} from "../../util/Timer";

import {Quality} from "../../model/write/Quality";

import {State, Dispatch} from "../../state/State";
import {WriteQuality as WriteQualityState} from "../../state/write/WriteQuality";
import {WriteStoryPart} from "../../state/write/WriteStoryPart";

import Loading from "../common/Loading";
import Icon from "../common/Icon";
import Link from "../common/Link";
import ScalingTextarea from "../common/ScalingTextarea";
import QualityHelp from "./QualityHelp";


function submit(dispatch: Dispatch, state: WriteQualityState) {
  return async(event: JSX.TargetedEvent<HTMLFormElement, Event>) => {
    event.preventDefault();

    dispatch(State.lens.page.write.part.writeStory.part.writeQuality.isSaving
      .set(true));

    let param = { text: state.raw };
    let req = state.urlname === null
        ? request<Quality>("POST",
            `/api/v1/write/stories/${state.story}/qualities`, param)
        : request<Quality>("PUT",
            `/api/v1/write/stories/${state.story}/qualities/${state.urlname}`,
            param);

    let response = await noEarlierThan(200, req);
    if (response.isError) {
      return;
    }

    dispatch(State.lens.page.write.part.writeStory.part.writeQuality.set({
      kind: "WriteQuality" as const,
      quality: response.message,
      story: state.story,
      raw: response.message.raw,
      urlname: response.message.urlname,
      isSaving: false
    }));
  };
}

function loadQuality(dispatch: Dispatch, state: WriteQualityState) {
  return async () => {
    let qualityResponse = await request<Quality>("GET",
      `api/v1/write/stories/${state.story}/qualities/${state.urlname}`);

    if (qualityResponse.isError) {
      return;
    }

    let quality = qualityResponse.message;

    dispatch(state => {
      let intermediate = State.lens.page.write.part.writeStory.part
        .writeQuality.quality
        .set(quality)(state);
      return State.lens.page.write.part.writeStory.part.writeQuality.raw
        .set(quality.raw)(intermediate);
    });
  };
}


function setText(dispatch: Dispatch) {
  return (event: JSX.TargetedEvent<HTMLTextAreaElement, Event>) => {
    dispatch(State.lens.page.write.part.writeStory.part.writeQuality.raw
      .set(event.currentTarget.value));
  };
}





export default function WriteQuality(state: WriteQualityState) {
  const dispatch = PreactHooks.useContext(DispatchContext);

  let title = state.urlname === null
      ? "Neue Qualität"
      : "Qualität bearbeiten";

  if (state.urlname !== null && state.quality === null) {
    useAsyncEffect(loadQuality(dispatch, state));
    return <Loading />;
  }

  let text = state.quality != null ? state.quality.raw : state.raw;

  let saveButton = state.isSaving
      ? <div type="submit" class="btn btn-primary disabled loading">
          <Icon name="empty-hourglass" class="mr-2" />
          Speichere…
        </div>
      : <button type="submit" class="btn btn-primary">
          <Icon name="save" class="mr-2" />
          Speichern
        </button>;

  let returnLink = State.lens.page.write.part.set(
    WriteStoryPart.instance(state.story));

  return (
    <div class="row">
      <form class="col-8" onSubmit={submit(dispatch, state)}>
        <h2>{title}</h2>

        <ScalingTextarea class="form-control larger"
          onChange={setText(dispatch)}
          content={text} />

        <div class="d-flex w-75 mx-auto mt-3 justify-content-between align-items-center">
          <Link target={returnLink}>Zurück</Link>

          {saveButton}
        </div>
      </form>
      <div class="col-4">
        <QualityHelp />
      </div>
    </div>
  );
}

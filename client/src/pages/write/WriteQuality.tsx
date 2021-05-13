import {Dispatch} from "../../core/Dispatch";
import {Lens} from "../../state/Pages";
import type {WriteQuality as State} from "../../state/page/write/WriteQuality";
import {WriteStory} from "../../state/page/write/WriteStory";

import type {Quality} from "../../model/write/Quality";
import type {Story} from "../../model/write/Story";

import QualityHelp from "../../components/write/QualityHelp";

import Icon from "../../components/common/Icon";
import Link from "../../components/common/Link";
import LoadableLoader from "../../components/common/LoadableLoader";
import ScalingTextarea from "../../components/common/ScalingTextarea";


function setText(newText: string) {
  return Dispatch.Update(Lens.WriteQuality.raw.set(newText));
}


function ErrorDisplay(state: { saveError: string | null }) {
  return state.saveError !== null
      ? <div class="alert alert-danger alert-raw my-3">{state.saveError}</div>
      : <></>;
}


function SaveButton(state: { isSaving: boolean }) {
  return state.isSaving
      ? <div type="submit" class="btn btn-primary disabled loading">
          <Icon name="empty-hourglass" class="mr-2" />
          Speichere…
        </div>
      : <button type="submit" class="btn btn-primary">
          <Icon name="save" class="mr-2" />
          Speichern
        </button>;
}


export function WriteQualityLoaded(state: State, story: Story, quality: Quality | null) {
  let returnLink = Dispatch.Page(WriteStory.page(story.urlname, story));
  let submit = (event: Event) => {
    event.preventDefault();
    state.save(state);
  };

  return (
    <div id="centre" class="px-0">
      <h1><Link action={returnLink}>Schreibe: {story.name}</Link></h1>

      <div class="row">
        <form class="col-8" onSubmit={submit}>
          <h2>{quality === null ? "Neue Szene" : quality.name}</h2>

          <ScalingTextarea class="form-control larger"
            onChange={setText}
            content={state.raw} />

          <ErrorDisplay saveError={state.saveError} />
          <div class="d-flex w-75 mx-auto mt-3 justify-content-between align-items-center">
            <Link action={returnLink}>Zurück</Link>
            <SaveButton isSaving={state.isSaving} />
          </div>
        </form>
        <div class="col-4">
          <QualityHelp />
        </div>
      </div>
    </div>
  );
}


export function WriteQuality(state: State) {
  return LoadableLoader(state.story, story => {
    if (state.quality !== null) {
      return LoadableLoader(state.quality, quality =>
          WriteQualityLoaded(state, story, quality));
    }
    else {
      return WriteQualityLoaded(state, story, null);
    }
  });
}

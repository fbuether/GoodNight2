import {Dispatch} from "../../core/Dispatch";
import {Lens} from "../../state/Pages";
import type {WriteQuality as State} from "../../state/page/write/WriteQuality";
import {WriteStory} from "../../state/page/write/WriteStory";

import type {Quality} from "../../state/model/write/Quality";
import type {Story} from "../../state/model/write/Story";

import QualityHelp from "../../components/write/QualityHelp";

import Icon from "../../components/common/Icon";
import Link from "../../components/common/Link";
import Error from "../../components/common/Error";
import SaveButton from "../../components/common/SaveButton";
import LoadableLoader from "../../components/common/LoadableLoader";
import ScalingTextarea from "../../components/common/ScalingTextarea";


function setText(newText: string) {
  return Dispatch.Update(Lens.WriteQuality.raw.set(newText));
}


export function WriteQualityLoaded(state: State, story: Story, quality: Quality | null) {
  let returnLink = Dispatch.Page(WriteStory.page(story.urlname, story));
  let submit = (event: Event) => {
    event.preventDefault();
    state.save(state);
  };

  let doDelete = (event: Event) => {
    event.preventDefault();
    state.onDelete(state);
  }


  return (
    <div id="centre" class="px-0">
      <h1><Link action={returnLink}>Schreibe: {story.name}</Link></h1>

      <div class="row">
        <form class="col-8" onSubmit={submit}>
          <h2>{quality === null ? "Neue Qualität" : quality.name}</h2>

          <ScalingTextarea class="form-control larger"
            onChange={setText}
            content={state.raw} />

          <Error message={state.saveError} />
          <div class="my-3 mx-4 d-flex justify-content-end g-4">
            <button class="btn btn-danger mx-3"
              onClick={doDelete}>
              <Icon name="tombstone" />
              Löschen
            </button>
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

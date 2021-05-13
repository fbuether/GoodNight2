import {Dispatch} from "../../core/Dispatch";
import {Lens} from "../../state/Pages";
import type {WriteScene as State} from "../../state/page/write/WriteScene";
import {WriteStory} from "../../state/page/write/WriteStory";

import type {Scene} from "../../model/write/Scene";
import type {Story} from "../../model/write/Story";

import SceneHelp from "../../components/write/SceneHelp";

import Icon from "../../components/common/Icon";
import Link from "../../components/common/Link";
import Error from "../../components/common/Error";
import SaveButton from "../../components/common/SaveButton";
import LoadableLoader from "../../components/common/LoadableLoader";
import ScalingTextarea from "../../components/common/ScalingTextarea";


function setText(newText: string) {
  return Dispatch.Update(Lens.WriteScene.raw.set(newText));
}


export function WriteSceneLoaded(state: State, story: Story, scene: Scene | null) {
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
          <h2>{scene === null ? "Neue Szene" : scene.name}</h2>

          <ScalingTextarea class="form-control larger"
            onChange={setText}
            content={state.raw} />

          <Error message={state.saveError} />
          <div class="d-flex w-75 mx-auto mt-3 justify-content-between align-items-center">
            <Link action={returnLink}>Zurück</Link>
            <SaveButton isSaving={state.isSaving} />
          </div>
        </form>
        <div class="col-4">
          <SceneHelp />
        </div>
      </div>
    </div>
  );
}


export function WriteScene(state: State) {
  return LoadableLoader(state.story, story => {
    if (state.scene !== null) {
      return LoadableLoader(state.scene, scene =>
          WriteSceneLoaded(state, story, scene));
    }
    else {
      return WriteSceneLoaded(state, story, null);
    }
  });
}

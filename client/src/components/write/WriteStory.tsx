import * as PreactHooks from "preact/hooks";
import DispatchContext from "../../DispatchContext";
import useAsyncEffect from "../../ui/useAsyncEffect";
import request from "../../Request";

import {State, Dispatch} from "../../state/State";
import {Story} from "../../model/write/Story";

import {SelectStoryPart} from "../../state/write/SelectStoryPart";
import {WriteStoryPart} from "../../state/write/WriteStoryPart";
import {StoryOverview} from "../../state/write/StoryOverview";
import {WriteScene} from "../../state/write/WriteScene";

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

  let toNewScene = State.lens.page.write.part.writeStory.part
    .set(WriteScene.instance);
  let toNewQuality = State.lens.page.write.part.writeStory.part
    .set(WriteScene.instance);

  return (
    <div id="centre" class="px-0">
      <h1><Link target={toBase}>Schreibe: {state.story.name}</Link></h1>

      <div class="d-flex justify-content-around mt-2 mb-3">
        <Link target={toNewScene} class="boxed">
          Neue Szene hinzufügen
        </Link>
        <Link target={toNewQuality} class="boxed">
          Neue Qualität hinzufügen
        </Link>
      </div>

      <div class="row">
        <div class="col-8">
          <h2>Inhalt</h2>

          <ul class="category">
            <li class="group"><div>Orte</div>
              <ul>
                <li class="link"><a href="#">Am Kreuzgang</a></li>

                <li class="group"><div>Hels Schlucht</div>
                  <ul>
                    <li class="link"><a href="#">Eingang</a></li>
                    <li class="link"><a href="#">Schmiede</a></li>
                  </ul>
                </li>
              </ul>
            </li>
          </ul>
        </div>
        <div class="col-4">
          <h2>Tags</h2>
          <ul class="tags list-unstyled list-inline">
            <li class="list-inline-item"><a href="#">Nora</a></li>
            <li class="list-inline-item"><a href="#">Untergang</a></li>
          </ul>
        </div>
      </div>
    </div>
  );
}

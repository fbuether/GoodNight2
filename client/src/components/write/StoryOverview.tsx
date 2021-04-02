import * as PreactHooks from "preact/hooks";
import DispatchContext from "../../DispatchContext";
import useAsyncEffect from "../../ui/useAsyncEffect";
import request from "../../Request";

import {Scene} from "../../model/write/Scene";
import {State, Dispatch} from "../../state/State";
import {WriteScene} from "../../state/write/WriteScene";
import {StoryOverview as StoryState} from "../../state/write/StoryOverview";

import Link from "../common/Link";
import Loading from "../common/Loading";


function loadScenes(dispatch: Dispatch, story: string) {
  return async () => {
    let response = await request<Array<Scene>>(
      "GET", `api/v1/write/story/${story}/scenes`);
    if (response.isError) {
      return;
    }

    dispatch(State.lens.page.write.part.writeStory.part.storyOverview
      .scenes.set(response.message));
  }
}


export default function StoryOverview(state: StoryState) {
  const dispatch = PreactHooks.useContext(DispatchContext);

  let scenes;
  if (state.scenes == null) {
    useAsyncEffect(loadScenes(dispatch, state.story));
    scenes = <Loading />;
  }
  else {
    let editScene = (s: Scene) =>
        State.lens.page.write.part.writeStory.part.set({
          ...WriteScene.instance(state.story),
          scene: s,
          urlname: s.urlname
        });

    scenes = (
      <ul class="category">
        <li class="group"><div>Alle Szenen</div>
          <ul>
            {state.scenes.map(s => (
              <li class="link">
                <Link target={editScene(s)}>{s.name}</Link>
              </li>
            ))}
          </ul>
        </li>
      </ul>
    );
  
          //       <li class="group"><div>Hels Schlucht</div>
          //         <ul>
          //           <li class="link"><a href="#">Eingang</a></li>
          //           <li class="link"><a href="#">Schmiede</a></li>
          //         </ul>
          //       </li>
          //     </ul>
          //   </li>
          // </ul>;
  }

  let toNewScene = State.lens.page.write.part.writeStory.part.set(
    WriteScene.instance(state.story));

  let toNewQuality = State.lens.page.write.part.writeStory.part
    .set(WriteScene.instance(state.story));

  return (
    <>
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
          {scenes}
        </div>
        <div class="col-4">
          <h2>Tags</h2>
          <ul class="tags list-unstyled list-inline">
            <li class="list-inline-item"><a href="#">Nora</a></li>
            <li class="list-inline-item"><a href="#">Untergang</a></li>
          </ul>
        </div>
      </div>
      </>
  );
}

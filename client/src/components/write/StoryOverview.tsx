
import {State, Dispatch} from "../../state/State";

import {WriteScene} from "../../state/write/WriteScene";
import {StoryOverview as StoryState} from "../../state/write/StoryOverview";

import Link from "../common/Link";


export default function StoryOverview(state: StoryState) {
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
      </>
  );
}

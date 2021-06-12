import {Dispatch} from "../../core/Dispatch";
import type {Story} from "../../state/model/write/Story";
import type {Loadable} from "../../state/Loadable";

import {WriteStory} from "../../state/page/write/WriteStory";
import {CreateStory} from "../../state/page/write/CreateStory";

import {SelectStory as State} from "../../state/page/write/SelectStory";

import ShowStories from "../stories/ShowStories";
import Icon from "../common/Icon";
import Link from "../common/Link";


export function SelectStory(state: State) {
  var page = (urlname: string) => WriteStory.page(urlname);
  var newStory = Dispatch.Page(CreateStory.page);

  return (
    <div id="centre" class="row px-0 g-0">
      <h1>Schreibe eine Geschichte</h1>
      <p>
        Wähle eine Geschichte, die du schreiben willst, oder erstelle eine
        neue Geschichte.
      </p>
      <ShowStories stories={state.stories} page={page} cols={3}>
        <div class="col">
          <div class="card new">
            <div class="card-body">
              <h5>
                <Icon name="sundial" class="top mr-2" />
                <Link class="stretched-link" action={newStory}>
                  Neue Geschichte…
                </Link>
              </h5>
              <p>Beginne eine neue Geschichte, ein neues Abenteuer.</p>
            </div>
          </div>
        </div>
      </ShowStories>
    </div>
  );
}

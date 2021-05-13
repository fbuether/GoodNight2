import {Dispatch} from "../../core/Dispatch";
import type {Story} from "../../model/write/Story";
import type {Loadable} from "../../state/Loadable";

import {WriteStory} from "../../state/page/write/WriteStory";
import {CreateStory} from "../../state/page/write/CreateStory";

import ShowStories from "../../components/read/ShowStories";
import Icon from "../../components/common/Icon";
import Link from "../../components/common/Link";


export interface SelectStory {
  page: "SelectStory";
  stories: Loadable<Array<Story>>;
}

export function SelectStory(state: SelectStory) {
  var page = (urlname: string) => WriteStory.page(urlname);
  var newStory = Dispatch.Page(CreateStory.page);

  return (
    <div id="centre" class="row px-0 g-0">
      <h1>Schreibe eine Geschichte</h1>
      <p>
        Wähle eine Geschichte, die du schreiben willst, oder erstelle eine
        neue Geschichte.
      </p>
      <ShowStories stories={state.stories} page={page}>
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

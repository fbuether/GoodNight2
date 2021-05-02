import {Dispatch} from "../../core/Dispatch";
import type {Story} from "../../model/write/Story";
import type {Loadable} from "../../state/Loadable";

import {WriteStory} from "../../state/page/write/WriteStory";

import ShowStories from "../../components/read/ShowStories";


export interface SelectStory {
  page: "SelectStory";
  stories: Loadable<Array<Story>>;
}

export function SelectStory(state: SelectStory) {
  var page = (urlname: string) => WriteStory.page(urlname);

  return (
    <div id="centre" class="row px-0 g-0">
      <h1>Schreibe eine Geschichte</h1>
      <p>
        Wähle eine Geschichte, die du schreiben willst, oder erstelle eine
        neue Geschichte.
      </p>
      <ShowStories stories={state.stories} page={page} />
    </div>
  );
}

      // <div class="row cards row-cols-1 row-cols-sm-2 row-cols-md-3 g-3 mt-0">
      //   <div class="col">
      //     <div class="card new">
      //       <div class="card-body">
      //         <h5>
      //           <Icon name="sundial" class="top mr-2" />
      //           <Link class="stretched-link" target={newLink}>
      //             Neue Geschichte…
      //           </Link>
      //         </h5>
      //         <p>Beginne eine neue Geschichte, ein neues Abenteuer.</p>
      //       </div>
      //     </div>
      //   </div>

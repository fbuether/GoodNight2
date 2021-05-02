import {Dispatch} from "../../core/Dispatch";
import type {Story} from "../../model/read/Story";

import {ReadStory} from "../../state/page/read/ReadStory";

import Link from "../common/Link";


export interface StoryCard {
  story: Story;
}


export default function StoryCard(state: StoryCard) {
  let action = Dispatch.Page(ReadStory.page(state.story.urlname));

  return (
    <div class="col">
      <div class="card">
        <div class="card-body">
          <h5>
            <Link class="stretched-link" action={action}>
              {state.story.name}
            </Link>
          </h5>
          <p>{state.story.description}</p>
        </div>
      </div>
    </div>
  );
}

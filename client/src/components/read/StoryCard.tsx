import {Dispatch} from "../../core/Dispatch";
import type {PageDescriptor} from "../../core/PageDescriptor";

import Link from "../common/Link";


export interface ShowableStory {
  name: string;
  description: string;
  urlname: string;
}

export interface StoryCard {
  story: ShowableStory;
  page: (urlname: string) => PageDescriptor;
}


export default function StoryCard(state: StoryCard) {
  let action = Dispatch.Page(state.page(state.story.urlname));

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

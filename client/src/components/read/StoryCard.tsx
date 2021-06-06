import {Dispatch} from "../../core/Dispatch";
import type {PageDescriptor} from "../../core/PageDescriptor";

import Link from "../common/Link";
import Icon from "../common/Icon";


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
            <Icon name="bookmarklet" class="mr-2 restrained middle" />
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

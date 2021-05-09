import {Dispatch} from "../../core/Dispatch";
import type {Story, Category} from "../../model/write/Story";
import type {Loadable} from "../../state/Loadable";

import {WriteStory as State} from "../../state/page/write/WriteStory";
import {WriteScene} from "../../state/page/write/WriteScene";

import {Category as CategoryC} from "../../components/write/Category";

import Link from "../../components/common/Link";
import Icon from "../../components/common/Icon";
import LoadableLoader from "../../components/common/LoadableLoader";



function WriteStoryInner(state: State, story: Story, category: Category) {
  let toNewScene = Dispatch.Page(WriteScene.pageNew(story.urlname));
  let toNewQuality = "http://localhost:32015";
  let toBase = Dispatch.Page(State.page(state.urlname, story));

  return (
    <div id="centre" class="px-0">
      <h1><Link action={toBase}>Schreibe: {story.name}</Link></h1>
      <div class="d-flex justify-content-around mt-2 mb-3">
        <Link action={toNewScene} class="boxed">
          <Icon class="restrained color-primary mr-1" name="horizon-road" />
          Neue Szene
        </Link>
        <Link action={toNewQuality} class="boxed">
          <Icon class="restrained color-secondary mr-1" name="swap-bag" />
          Neue Qualität
        </Link>
      </div>

      <div class="row">
        <div class="col-8">
          <h2>Inhalt</h2>
          <CategoryC story={story.urlname} category={category}
            withHeader={false} />
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

export function WriteStory(state: State) {
  return LoadableLoader(state.story, story =>
      LoadableLoader(state.category, category =>
          WriteStoryInner(state, story, category)));
}

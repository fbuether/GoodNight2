import * as PreactHooks from "preact/hooks";
import DispatchContext from "../../DispatchContext";
import useAsyncEffect from "../../ui/useAsyncEffect";
import request from "../../Request";

import {Scene} from "../../model/write/Scene";
import {Quality} from "../../model/write/Quality";
import {Story, Category} from "../../model/write/Story";
import {State, Dispatch} from "../../state/State";
import {WriteScene} from "../../state/write/WriteScene";
import {WriteQuality} from "../../state/write/WriteQuality";
import {StoryOverview as StoryState} from "../../state/write/StoryOverview";

import Link from "../common/Link";
import Icon from "../common/Icon";
import Loading from "../common/Loading";


function loadScenes(dispatch: Dispatch, story: string) {
  return async () => {
    let response = await request<Category>(
      "GET", `api/v1/write/stories/${story}/content-by-category`);
    if (response.isError) {
      return;
    }

    dispatch(State.lens.page.write.part.writeStory.part.storyOverview
      .categories.set(response.message));
  };
}


function renderCategory(storyUrlname: string, category: Category, withHeader: boolean): JSX.Element {
  let editScene = (s: Scene) =>
      State.lens.page.write.part.writeStory.part.set({
        ...WriteScene.instance(storyUrlname),
        scene: s,
        raw: s.raw,
        urlname: s.urlname
      });

  let editQuality = (q: Quality) =>
      State.lens.page.write.part.writeStory.part.set({
        ...WriteQuality.instance(storyUrlname),
        quality: q,
        raw: q.raw,
        urlname: q.urlname
      });

  let name = category.name != ""
      ? <li>{category.name}</li>
      : <></>;

  let scenes = category.scenes.map(s => (
    <li class="link s">
      <Link target={editScene(s)}>
        <Icon class="restrained color-primary mr-1" name="horizon-road" />
        {s.name}
      </Link>
    </li>));

  let qualities = category.qualities.map(q => (
    <li class="link q">
      <Link target={editQuality(q)}>
        <Icon class="restrained color-secondary mr-1" name="swap-bag" />
        {q.name}
      </Link>
    </li>));

  let categories = category.categories.map(c =>
      renderCategory(storyUrlname, c, true));

  if (withHeader) {
    return (
      <ul class="category">
        <li class="group">
          <div>{category.name}</div>
          <ul>{scenes}{qualities}</ul>
          {categories}
        </li>
      </ul>
    );
  }
  else {
    return (<>
        <ul class="category">{scenes}{qualities}</ul>
        {categories}
      </>);
  }
}


export default function StoryOverview(state: StoryState) {
  const dispatch = PreactHooks.useContext(DispatchContext);

  if (state.categories == null) {
    useAsyncEffect(loadScenes(dispatch, state.story));
  }

  let scenes = state.categories == null
      ? <Loading />
      : renderCategory(state.story, state.categories, false);

  let toNewScene = State.lens.page.write.part.writeStory.part.set(
    WriteScene.instance(state.story));

  let toNewQuality = State.lens.page.write.part.writeStory.part
    .set(WriteQuality.instance(state.story));

  return (
    <>
      <div class="d-flex justify-content-around mt-2 mb-3">
        <Link target={toNewScene} class="boxed">
          <Icon class="restrained color-primary mr-1" name="horizon-road" />
          Neue Szene
        </Link>
        <Link target={toNewQuality} class="boxed">
          <Icon class="restrained color-secondary mr-1" name="swap-bag" />
          Neue Qualität
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

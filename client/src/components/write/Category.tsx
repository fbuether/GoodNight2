import {Dispatch} from "../../core/Dispatch";

import type {Quality} from "../../state/model/write/Quality";
import type {Scene as Scene} from "../../state/model/write/Scene";
import type {Story, Category as CategoryT} from "../../state/model/write/Story";

import {WriteScene} from "../../state/page/write/WriteScene";
import {WriteQuality} from "../../state/page/write/WriteQuality";

import Icon from "../common/Icon";
import Link from "../common/Link";
import Tags from "./Tags";


export interface Category {
  story: Story;
  category: CategoryT;
  withHeader: boolean;
}



function SceneLink(story: Story, scene: Scene) {
  let editScene = Dispatch.Page(WriteScene.page(story.urlname, scene.urlname));

  return (
    <li class="link s">
      <Link action={editScene}>
        <Icon class="restrained color-primary mr-1" name="horizon-road" />
        {scene.name}
      </Link>
      {Tags(story, scene.tags)}
    </li>
  );
}

function QualityLink(story: Story, quality: Quality) {
  let editQuality = Dispatch.Page(WriteQuality.page(story.urlname, quality.urlname));

  return (
    <li class="link q">
      <Link action={editQuality}>
        <Icon class="restrained color-secondary mr-1" name="swap-bag" />
        {quality.name}
      </Link>
      {Tags(story, quality.tags)}
    </li>
  );
}


export function Category(state: Category): JSX.Element {
  let scenes = state.category.scenes.map(s => SceneLink(state.story, s));
  let qualities = state.category.qualities.map(q => QualityLink(state.story, q));

  let categories = state.category.categories.map(c =>
      Category({ story: state.story, category: c, withHeader: true }));

  let content = (<>
        <ul class="category">{scenes}{qualities}</ul>
        {categories}
      </>);


  if (!state.withHeader) {
    return content;
  }

  return (
    <ul class="category">
      <li class="group">
        <div>{state.category.name}</div>
        {content}
      </li>
    </ul>
  );
}


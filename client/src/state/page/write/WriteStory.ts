import type {State} from "../../State";
import {Lens} from "../../Pages";
import {Dispatch} from "../../../core/Dispatch";
import type {PageDescriptor} from "../../../core/PageDescriptor";

import type {Story, Category} from "../../../model/write/Story";
import {Loadable} from "../../Loadable";


import {WriteStory as Component} from "../../../pages/write/WriteStory";
export type WriteStory = Component;



async function onLoad(dispatch: Dispatch, state: State) {
  var storyUrlname = Lens.WriteStory.urlname.get(state.page);
  if (storyUrlname == null) {
    throw "Invalid state: OnLoad for wrong page.";
  }

  let story = Loadable.forRequest<Story>(
    dispatch, state,
    "GET", `api/v1/write/stories/${storyUrlname}`,
    Lens.WriteStory.story);

  let categories = Loadable.forRequest<Array<Category>>(
    dispatch, state,
    "GET", `api/v1/write/stories/${storyUrlname}/content-by-category`,
    Lens.WriteStory.categories);

  await Promise.all([story, categories]);
}


function instance(urlname: string): WriteStory {
  return {
    page: "WriteStory" as const,
    urlname: urlname,
    story: Loadable.Unloaded,
    categories: Loadable.Unloaded
  };
}

function page(urlname: string): PageDescriptor {
  return {
    state: instance(urlname),
    url: "/write/" + urlname,
    title: "GoodNight: Schreibe",
    onLoad: onLoad,
    render: () => Component(instance(urlname))
  };
}

export const WriteStory = {
  path: /^\/write\/([^\/]+)$/,
  page: page,
  ofUrl: (pathname: string, matches: Array<string>) => WriteStory.page(matches[1])
}

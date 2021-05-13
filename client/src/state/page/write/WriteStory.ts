import type {State} from "../../State";
import {Lens} from "../../Pages";
import {Dispatch} from "../../../core/Dispatch";
import type {PageDescriptor} from "../../../core/PageDescriptor";

import type {Story, Category} from "../../../model/write/Story";
import {Loadable} from "../../Loadable";


export interface WriteStory {
  page: "WriteStory";
  urlname: string;
  story: Loadable<Story>;
  category: Loadable<Category>;
}



async function onLoad(dispatch: Dispatch, state: State) {
  var storyUrlname = Lens.WriteStory.urlname.get(state.page);
  if (storyUrlname == null) {
    throw "Invalid state: OnLoad for wrong page.";
  }

  let story = Loadable.forRequest<Story>(
    dispatch, state,
    "GET", `api/v1/write/stories/${storyUrlname}`,
    Lens.WriteStory.story);

  let category = Loadable.forRequest<Category>(
    dispatch, state,
    "GET", `api/v1/write/stories/${storyUrlname}/content-by-category`,
    Lens.WriteStory.category);

  await Promise.all([story, category]);
}


function instance(urlname: string, story?: Story): WriteStory {
  return {
    page: "WriteStory" as const,
    urlname: urlname,
    story: story ? Loadable.Loaded(story) : Loadable.Unloaded,
    category: Loadable.Unloaded
  };
}

function page(urlname: string, story?: Story): PageDescriptor {
  return {
    state: instance(urlname, story),
    url: "/write/stories/" + urlname,
    title: "GoodNight: Schreibe" + (story ? " "+ story.name : ""),
    onLoad: onLoad
  };
}

export const WriteStory = {
  path: /^\/write\/stories\/([^\/]+)$/,
  page: page,
  ofUrl: (pathname: string, matches: Array<string>) => WriteStory.page(matches[1])
};

import {request} from "../../../service/RequestService";
import {Dispatch} from "../../../core/Dispatch";
import type {PageDescriptor} from "../../../core/PageDescriptor";
import type {State} from "../../State";
import type {Page} from "../../Page";
import {Pages, Lens} from "../../Pages";

import type {Story} from "../../../model/read/Story";

import {Loadable, StoryOverview as Component} from "../../../pages/read/StoryOverview";
export type StoryOverview = Component;



async function onLoad(dispatch: Dispatch, state: State) {

  var loadingState = Lens.StoryOverview.stories.state.get(state.page);

  if (loadingState == "unloaded") {
    dispatch(Dispatch.Update(Lens.StoryOverview.stories.set({ state: "loading" })));

    var storiesResponse = await request<Array<Story>>(
      "GET", "api/v1/read/stories");

    if (storiesResponse.isResult) {
      dispatch(Dispatch.Update(Lens.StoryOverview.stories.set({ state: "loaded", result: storiesResponse.message })));
    }
    else {
      dispatch(Dispatch.Update(Lens.StoryOverview.stories.set({ state: "failed", error: storiesResponse.message })));
    }
  }
}


function instance(stories: Loadable<Array<Story>>) {
  return {
    page: "StoryOverview" as const,
    stories: stories
  };
}

function page(stories: Loadable<Array<Story>>): PageDescriptor {
  return {
    state: instance(stories),
    url: "/read",
    title: "GoodNight: Übersicht der Geschichten",
    onLoad: onLoad,
    render: () => Component(instance(stories))
  };
}

export const StoryOverview = {
  path: /^\/read$/,
  page: page({ state: "unloaded" }),
  ofUrl: (pathname: string, matches: Array<string>) => page({ state: "unloaded" })
};

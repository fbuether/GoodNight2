import {request} from "../../../service/RequestService";
import {Dispatch} from "../../../core/Dispatch";
import type {PageDescriptor} from "../../../core/PageDescriptor";
import type {State} from "../../State";
import {Lens} from "../../Pages";

import type {Story} from "../../../model/read/Story";
import {Loadable} from "../../Loadable";

import {StoryOverview as Component} from "../../../pages/read/StoryOverview";
export type StoryOverview = Component;



async function onLoad(dispatch: Dispatch, state: State) {
  await Loadable.forRequest<Array<Story>>(state,
    "GET", "api/v1/read/stories",
    Lens.StoryOverview.stories);
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
    onLoad: onLoad
  };
}

export const StoryOverview = {
  path: /^\/read\/?$/,
  page: page(Loadable.Unloaded),
  ofUrl: (pathname: string, matches: Array<string>) => page(Loadable.Unloaded)
};

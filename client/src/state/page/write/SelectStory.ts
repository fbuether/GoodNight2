import type {State} from "../../State";
import {Lens} from "../../Pages";
import {Dispatch} from "../../../core/Dispatch";
import type {PageDescriptor} from "../../../core/PageDescriptor";

import type {Story} from "../../../model/write/Story";
import {Loadable} from "../../Loadable";


import {SelectStory as Component} from "../../../pages/write/SelectStory";
export type SelectStory = Component;



async function onLoad(dispatch: Dispatch, state: State) {
  await Loadable.forRequest<Array<Story>>(
    dispatch, state,
    "GET", "api/v1/write/stories",
    Lens.SelectStory.stories);
}


function instance(stories: Loadable<Array<Story>>) {
  return {
    page: "SelectStory" as const,
    stories: stories
  };
}

function page(stories: Loadable<Array<Story>>): PageDescriptor {
  return {
    state: instance(stories),
    url: "/write",
    title: "GoodNight: Geschichten schreiben",
    onLoad: onLoad,
    render: () => Component(instance(stories))
  };
}

export const SelectStory = {
  path: /^\/write\/?$/,
  page: page(Loadable.Unloaded),
  ofUrl: (pathname: string, matches: Array<string>) => SelectStory.page
}

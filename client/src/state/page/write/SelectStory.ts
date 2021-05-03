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


const instance = ({
  page: "SelectStory" as const,
  stories: Loadable.Unloaded
});

const page: PageDescriptor = ({
  state: instance,
  url: "/write",
  title: "GoodNight: Geschichten schreiben",
  onLoad: onLoad
});


export const SelectStory = {
  path: /^\/write\/?$/,
  page: page,
  ofUrl: (pathname: string, matches: Array<string>) => SelectStory.page
}

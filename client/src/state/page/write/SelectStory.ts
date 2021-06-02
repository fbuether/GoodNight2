import type {State} from "../../State";
import {Lens} from "../../Pages";
import {Dispatch} from "../../../core/Dispatch";
import {PageDescriptor, registerPageMapper} from "../../../core/PageDescriptor";

import type {Story} from "../../../model/write/Story";
import {Loadable} from "../../Loadable";


import {SelectStory as Component} from "../../../pages/write/SelectStory";
export type SelectStory = Component;



async function onLoad(dispatch: Dispatch, state: State) {
  await Loadable.forRequest<Array<Story>>(state,
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
  onLoad: onLoad,
  requiresAuth: true
});


export const SelectStory = {
  page: page
}

registerPageMapper(/^\/write\/?$/, page);

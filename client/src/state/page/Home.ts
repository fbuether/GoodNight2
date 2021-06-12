import {PageDescriptor, registerPageMapper} from "../../core/PageDescriptor";
import {Dispatch} from "../../core/Dispatch";
import type {State} from "../State";
import {Lens} from "../Pages";
import {Loadable} from "../Loadable";

import type {Story} from "../model/read/Story";


export interface Home {
  page: "Home";
  ownStories: Loadable<Array<Story>>;
}


async function onLoad(dispatch: Dispatch, state: State) {
  if (state.user.kind == "SignedIn") {
    await Loadable.forRequest<Array<Story>>(state,
      "GET", "api/v1/read/user/stories/mine",
      Lens.Home.ownStories);
  }
  else {
    Dispatch.send(Dispatch.Update(Lens.Home.ownStories.set(
      Loadable.Failed("User not logged in"))));
  }
}


const instance = {
  page: "Home" as const,
  ownStories: Loadable.Unloaded
};

const page: PageDescriptor = {
  state: instance,
  url: "/",
  title: "GoodNight",
  onLoad: onLoad
};

export const Home = {
  page: page
};


registerPageMapper(/^$/, page);

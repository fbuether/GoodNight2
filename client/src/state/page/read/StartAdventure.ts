import {request} from "../../../service/RequestService";
import {Dispatch} from "../../../core/Dispatch";
import {PageDescriptor, registerPageMapper} from "../../../core/PageDescriptor";
import type {State} from "../../State";
import {Lens} from "../../Pages";

import {Loadable, LoadableP} from "../../Loadable";

import type {Story} from "../../model/read/Story";
import type {Adventure} from "../../model/read/Adventure";

import {ReadStory} from "./ReadStory";

export interface StartAdventure {
  page: "StartAdventure";
  story: LoadableP<string,Story>;
  name: string;
  onStart: (state: StartAdventure) => Promise<void>;
  isStarting: boolean;
  error: string | null;
}


async function onStart(state: StartAdventure) {
  let storyLoader = state.story;
  if (storyLoader.state != "loaded") {
    throw "StartAdventure.onStart without loaded story.";
  }

  let story = storyLoader.result;

  let param = { name: state.name };
  let response = await request<Adventure>(
    "POST", `api/v1/read/stories/${story.urlname}/start`, param);

  if (response.isResult) {
    Dispatch.send(Dispatch.Page(ReadStory.page(
      story.urlname, response.message)));
  }
  else {
    let err = response.message;
    Dispatch.send(Dispatch.Update(pages => {
      let p2 = Lens.StartAdventure.error.set(err)(pages);
      return Lens.StartAdventure.isStarting.set(false)(p2);
    }));
  }
}


async function onLoad(dispatch: Dispatch, state: State) {
  var storyUrlname = Lens.StartAdventure.story.get(state.page);
  await Loadable.forRequestP<string,Story>(state,
    "GET", (story: string) => `api/v1/read/stories/${story}`,
    Lens.StartAdventure.story);
}


function page(storyUrlname: string): PageDescriptor {
  return {
    state: {
      page: "StartAdventure" as const,
      story: Loadable.UnloadedP(storyUrlname),
      name: "",
      onStart: onStart,
      isStarting: false,
      error: null
    },
    url: "/read/" + storyUrlname + "/start",
    title: "GoodNight: Lesen",
    onLoad: onLoad,
    requiresAuth: true
  };
}

export const StartAdventure = {
  page: page
};

registerPageMapper(/^\/read\/([^\/]+)\/start$/,
  (matches: ReadonlyArray<string>) => StartAdventure.page(matches[1]));

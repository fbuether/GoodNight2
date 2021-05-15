import {request} from "../../../service/RequestService";
import {Dispatch} from "../../../core/Dispatch";
import type {PageDescriptor} from "../../../core/PageDescriptor";
import type {State} from "../../State";
import {Lens} from "../../Pages";
import {Loadable, LoadableP} from "../../Loadable";

import type {Adventure} from "../../../model/read/Adventure";
import type {Story} from "../../../model/read/Story";
import type {Action, Option} from "../../../model/read/Action";
import {StartAdventure} from "./StartAdventure";


export interface ActionState {
  action: Action;
  onOption: (optionUrlname: string) => Promise<void>;
}

export interface OptionState {
  option: Option;
  onOption: (optionUrlname: string) => Promise<void>;
}


export interface ReadStory {
  page: "ReadStory";
  story: LoadableP<string, Story>;
  adventure: Loadable<Adventure>;
  onOption: (state: ReadStory, optionUrlname: string) => Promise<void>;
}


async function onOption(state: ReadStory, optionUrlname: string) {
  console.log("option", optionUrlname);
}


async function onLoad(dispatch: Dispatch, state: State) {
  var storyUrlname = Lens.ReadStory.story.unloaded.value.get(state.page);

  var storyLoader = Loadable.forRequestP<string,Story>(state,
    "GET", (key: string) => `api/v1/read/stories/${key}`,
    Lens.ReadStory.story);
  var advLoader = Loadable.forRequest<Adventure>(state,
    "GET", `api/v1/read/stories/${storyUrlname}/continue`,
    Lens.ReadStory.adventure);
  await Promise.all([storyLoader, advLoader]);

  Dispatch.send(Dispatch.Continue(state => {
    var advState = Lens.ReadStory.adventure.state.get(state.page);
    return advState !== "loaded" && storyUrlname !== null
        ? Dispatch.Page(StartAdventure.page(storyUrlname))
        : null;
  }));
}


function instance(urlname: string, adventure?: Adventure): ReadStory {
  return {
    page: "ReadStory" as const,
    story: Loadable.UnloadedP(urlname),
    adventure: adventure ? Loadable.Loaded(adventure) : Loadable.Unloaded,
    onOption: onOption
  };
}

function page(urlname: string, adventure?: Adventure): PageDescriptor {
  return {
    state: instance(urlname),
    url: "/read/" + urlname,
    title: "GoodNight: Lesen",
    onLoad: onLoad
  };
}


export const ReadStory = {
  path: /^\/read\/([^\/]+)$/,

  page: page,

  ofUrl: (pathname: string, matches: Array<string>) => page(matches[1])
};

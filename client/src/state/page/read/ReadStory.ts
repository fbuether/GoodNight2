import {request} from "../../../service/RequestService";
import {Dispatch} from "../../../core/Dispatch";
import type {PageDescriptor} from "../../../core/PageDescriptor";
import type {State} from "../../State";
import {Lens} from "../../Pages";

import {Loadable} from "../../Loadable";

import type {Adventure} from "../../../model/read/Adventure";


export interface ReadStory {
  page: "ReadStory";
  story: string; // urlname
  adventure: Loadable<Adventure>;
}


async function onLoad(dispatch: Dispatch, state: State) {
  var storyUrlname = Lens.ReadStory.story.get(state.page);
  await Loadable.forRequest<Adventure>(
    dispatch, state,
    "GET", `api/v1/read/stories/${storyUrlname}/continue`,
    Lens.ReadStory.adventure);
}




function instance(urlname: string, adventure?: Adventure): ReadStory {
  return {
    page: "ReadStory" as const,
    story: urlname,
    adventure: Loadable.Unloaded
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

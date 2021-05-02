import * as P from "../../../util/ProtoLens";

import {request} from "../../../service/RequestService";
import {Dispatch} from "../../../core/Dispatch";
import type {PageDescriptor} from "../../../core/PageDescriptor";
import type {State} from "../../State";
import {Lens} from "../../Pages";

import {Loadable} from "../../Loadable";

import type {Story} from "../../../model/read/Story";


import {ReadStory as Component} from "../../../pages/read/ReadStory";
export type ReadStory = Component;


async function onLoad(dispatch: Dispatch, state: State) {
  var urlname = Lens.ReadStory.urlname.get(state.page);

  await Loadable.forRequest<Story>(
    dispatch, state,
    "GET", `api/v1/read/stories/${urlname}`,
    Lens.ReadStory.story);
}




function instance(urlname: string): ReadStory {
  return {
    page: "ReadStory" as const,
    urlname: urlname,
    story: Loadable.Unloaded
  };
}

function page(urlname: string): PageDescriptor {
  return {
    state: instance(urlname),
    url: "/read/" + urlname,
    title: "GoodNight: Lesen",
    onLoad: onLoad,
    render: () => Component(instance(urlname))
  };
}


export const ReadStory = {
  path: /^\/read\/([^\/]+)$/,

  page: page,

  ofUrl: (pathname: string, matches: Array<string>) => page(matches[1])
}

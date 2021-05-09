import {Dispatch} from "../../../core/Dispatch";
import type {State} from "../../State";
import type {PageDescriptor} from "../../../core/PageDescriptor";

import type {Quality} from "../../../model/write/Quality";
import type {Story} from "../../../model/write/Story";

import {Loadable} from "../../Loadable";



export interface WriteQuality {
  page: "WriteQuality";
  story: Loadable<Story>;
  quality: Loadable<Quality>;

  isNew: boolean;
  isSaving: boolean;
  save: (state: WriteQuality) => Promise<void>;
}


function onLoad(storyUrlname: string, qualityUrlname?: string) {
  return async (dispatch: Dispatch, state: State) => {
    return;
  };
}

async function onSave(state: WriteQuality) {
  
}


function instance(storyUrlname: string, qualityUrlname: string): WriteQuality {
  return {
    page: "WriteQuality" as const,
    story: Loadable.Unloaded,
    quality: Loadable.Unloaded,
    isNew: qualityUrlname === undefined,
    isSaving: false,
    save: onSave
  };
}

function page(storyUrlname: string, qualityUrlname: string): PageDescriptor {
  return {
    state: instance(storyUrlname, qualityUrlname),
    url: "/write/" + storyUrlname + "/"
        + (qualityUrlname ? "/quality/" + qualityUrlname : "/new-quality"),
    title: "GoodNight: Neue Szene",
    onLoad: onLoad(storyUrlname, qualityUrlname)
  };
}



export const WriteQuality = {
  path: /^\/write\/([^\/]+)\/(quality\/(^\/+)|new-quality)$/,
  page: page,
  ofUrl: (pathname: string, matches: Array<string>) =>
      page(matches[1], matches[2])
}

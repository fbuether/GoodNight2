import {Dispatch} from "../../../core/Dispatch";
import {Lens} from "../../Pages";
import type {State} from "../../State";
import type {PageDescriptor} from "../../../core/PageDescriptor";
import {request} from "../../../service/RequestService";

import type {Quality} from "../../../model/write/Quality";
import type {Story} from "../../../model/write/Story";

import {Loadable, LoadableP} from "../../Loadable";



export interface WriteQuality {
  page: "WriteQuality";
  story: LoadableP<string,Story>;
  quality: LoadableP<[string,string],Quality> | null;
  raw: string;

  isSaving: boolean;
  saveError: string | null;
  save: (state: WriteQuality) => Promise<void>;
}


async function onLoad(dispatch: Dispatch, state: State) {
  let storyFetcher = Loadable.forRequestP<string, Story>(state,
    "GET", (story: string) => `api/v1/write/stories/${story}`,
    Lens.WriteQuality.story);

  // only try to fetch the quality if we have a quality.
  let qualityFetcher = Promise.resolve();
  if (Lens.WriteQuality.quality.value.get(state.page) !== null) {
    qualityFetcher = Loadable.forRequestP<[string,string],Quality>(state,
      "GET", (sq: [string,string]) =>
        `api/v1/write/stories/${sq[0]}/qualities/${sq[1]}`,
      Lens.WriteQuality.quality.value);
  }


  await Promise.all([storyFetcher, qualityFetcher]);

  Dispatch.send(Dispatch.Update(pages => {
    let quality = Lens.WriteQuality.quality.value.loaded.result.get(pages);
    if (quality == null) {
      return null;
    }

    return Lens.WriteQuality.raw.set(quality.raw)(pages);
  }));
}

async function onSave(state: WriteQuality) {
  Dispatch.send(Dispatch.Update(Lens.WriteQuality.isSaving.set(true)));

  let param = { text: state.raw };
  let method = state.quality === null ? "POST" as const : "PUT" as const;

  if (state.story.state != "loaded") {
    throw `Story in WriteQuality has invalid state: ${state.story.state}.`;
  }

  let story = state.story.result;
  let url = `/api/v1/write/stories/${story.urlname}/qualities`;

  if (state.quality !== null) {
    if (state.quality.state != "loaded") {
      throw `Quality state in WriteQuality is invalid: ${state.quality.state}.`;
    }

    let quality = state.quality.result;
    url = url + "/" + quality.urlname;
  }

  let response = await request<Quality>(method, url, param);

  if (response.isResult) {
    Dispatch.send(Dispatch.Page(loadedPage(story, response.message)));
  }
  else {
    Dispatch.send(Dispatch.Update(Lens.set({
      ...state,
      saveError: response.message,
      isSaving: false
    })));
  }
}


function instance(storyUrlname: string, qualityUrlname: string | null)
: WriteQuality {
  return {
    page: "WriteQuality" as const,
    story: Loadable.UnloadedP(storyUrlname),
    quality: qualityUrlname === null
        ? null
        : Loadable.UnloadedP([storyUrlname, qualityUrlname]),
    raw: "",
    isSaving: false,
    saveError: null,
    save: onSave
  };
}

function loadedPage(story: Story, quality: Quality): PageDescriptor {
  return {
    state: {
      page: "WriteQuality" as const,
      story: Loadable.Loaded(story),
      quality: Loadable.Loaded(quality),
      raw: quality.raw,
      isSaving: false,
      saveError: null,
      save: onSave
    },
    url: "/write/stories/" + story.urlname + "/quality/" + quality.urlname,
    title: "GoodNight: Szene bearbeiten",
    onLoad: onLoad
  };
}

function page(storyUrlname: string, qualityUrlname: string | null)
: PageDescriptor {
  return {
    state: instance(storyUrlname, qualityUrlname),
    url: "/write/stories/" + storyUrlname
        + (qualityUrlname !== null
            ? "/quality/" + qualityUrlname
            : "/new-quality"),
    title: "GoodNight: "
        + (qualityUrlname !== null
           ? "Qualität bearbeiten"
           : "Neue Qualität"),
    onLoad: onLoad
  };
}



export const WriteQuality = {
  path: /^\/write\/stories\/([^\/]+)\/(quality\/([^\/]+)|new-quality)$/,
  page: page,
  pageNew: (storyUrlname: string) => page(storyUrlname, null),
  ofUrl: (pathname: string, matches: Array<string>) =>
      page(matches[1], matches[3] === undefined ? null : matches[3])
};

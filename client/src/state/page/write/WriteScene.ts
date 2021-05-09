import {Dispatch} from "../../../core/Dispatch";
import {Lens} from "../../Pages";
import type {State} from "../../State";
import type {PageDescriptor} from "../../../core/PageDescriptor";
import {request} from "../../../service/RequestService";

import type {Scene} from "../../../model/write/Scene";
import type {Story} from "../../../model/write/Story";

import {Loadable, LoadableP} from "../../Loadable";



export interface WriteScene {
  page: "WriteScene";
  story: LoadableP<string, Story>;
  scene: LoadableP<[string, string], Scene>;
  raw: string;

  isNew: boolean;
  isSaving: boolean;
  saveError: string | null;
  save: (state: WriteScene) => Promise<void>;
}


async function onLoad(dispatch: Dispatch, state: State) {
  let storyFetcher = Loadable.forRequestP<string,Story>(
    dispatch, state,
    "GET", (story: string) => `api/v1/write/stories/${story}`,
    Lens.WriteScene.story);

  let sceneFetcher = Loadable.forRequestP<[string,string],Scene>(
    dispatch, state,
    "GET", (scene: [string,string]) =>
        `api/v1/write/stories/${scene[0]}/scenes/${scene[1]}`,
    Lens.WriteScene.scene);

  await Promise.all([storyFetcher, sceneFetcher]);

  Dispatch.send(Dispatch.Update(pages => {
    let scene = Lens.WriteScene.scene.loaded.result.get(pages);
    if (scene == null) {
      return null;
    }

    return Lens.WriteScene.raw.set(scene.raw)(pages);
  }));
}


async function onSave(state: WriteScene) {
  Dispatch.send(Dispatch.Update(Lens.WriteScene.isSaving.set(true)));

  let param = { text: state.raw };
  let method = state.isNew ? "POST" as const : "PUT" as const;

  if (state.story.state != "loaded") {
    throw `Story in WriteScene state has invalid state: ${state.story.state}.`;
  }

  let story = state.story.result;
  let url = `/api/v1/write/stories/${story.urlname}/scenes`;

  if (!state.isNew) {
    if (state.scene.state != "loaded") {
      throw `Scene in WriteScene has invalid state: ${state.scene.state}.`;
    }

    let scene = state.scene.result;
    url = url + "/" + scene.urlname;
  }

  let response = await request<Scene>(method, url, param);

  if (response.isResult) {
    Dispatch.send(Dispatch.Update(Lens.set({
      ...state,
      scene: Loadable.Loaded(response.message),
      raw: response.message.raw,
      isSaving: false,
      saveError: null,
      isNew: false
    })));
  }
  else {
    Dispatch.send(Dispatch.Update(Lens.set({
      ...state,
      saveError: response.message,
      isSaving: false
    })));
  }
}


function instance(storyUrlname: string, sceneUrlname: string): WriteScene {
  return {
    page: "WriteScene" as const,
    story: Loadable.UnloadedP(storyUrlname),
    scene: Loadable.UnloadedP([storyUrlname, sceneUrlname]),
    raw: "",
    isNew: sceneUrlname === undefined,
    isSaving: false,
    saveError: null,
    save: onSave
  };
}

function page(storyUrlname: string, sceneUrlname: string): PageDescriptor {
  return {
    state: instance(storyUrlname, sceneUrlname),
    url: "/write/" + storyUrlname
        + (sceneUrlname ? "/scene/" + sceneUrlname : "/new-scene"),
    title: "GoodNight: Neue Szene",
    onLoad: onLoad
  };
}



export const WriteScene = {
  path: /^\/write\/([^\/]+)\/(scene\/([^\/]+)|new-scene)$/,
  page: page,
  ofUrl: (pathname: string, matches: Array<string>) =>
      page(matches[1], matches[3])
}

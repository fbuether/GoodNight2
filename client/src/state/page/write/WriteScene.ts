import {Dispatch} from "../../../core/Dispatch";
import type {State} from "../../State";
import type {PageDescriptor} from "../../../core/PageDescriptor";

import type {Scene} from "../../../model/write/Scene";
import type {Story} from "../../../model/write/Story";

import {Loadable} from "../../Loadable";



export interface WriteScene {
  page: "WriteScene";
  story: Loadable<Story>;
  scene: Loadable<Scene>;

  isNew: boolean;
  isSaving: boolean;
  save: (state: WriteScene) => Promise<void>;
}


function onLoad(storyUrlname: string, sceneUrlname?: string) {
  return async (dispatch: Dispatch, state: State) => {
    return;
  };
}

async function onSave(state: WriteScene) {
  console.log("savin.");
}


function instance(storyUrlname: string, sceneUrlname: string): WriteScene {
  return {
    page: "WriteScene" as const,
    story: Loadable.Unloaded,
    scene: Loadable.Unloaded,
    isNew: sceneUrlname === undefined,
    isSaving: false,
    save: onSave
  };
}

function page(storyUrlname: string, sceneUrlname: string): PageDescriptor {
  return {
    state: instance(storyUrlname, sceneUrlname),
    url: "/write/" + storyUrlname + "/"
        + (sceneUrlname ? "/scene/" + sceneUrlname : "/new-scene"),
    title: "GoodNight: Neue Szene",
    onLoad: onLoad(storyUrlname, sceneUrlname)
  };
}



export const WriteScene = {
  path: /^\/write\/([^\/]+)\/(scene\/(^\/+)|new-scene)$/,
  page: page,
  ofUrl: (pathname: string, matches: Array<string>) =>
      page(matches[1], matches[2])
}

import {Dispatch} from "../../../core/Dispatch";
import {Lens} from "../../Pages";
import type {State} from "../../State";
import {PageDescriptor, registerPageMapper} from "../../../core/PageDescriptor";
import {request, isResult, ResultResponse} from "../../../service/RequestService";
import {Loadable, LoadableP} from "../../Loadable";

import type {Scene} from "../../model/write/Scene";
import type {Story} from "../../model/write/Story";
import {WriteStory} from "./WriteStory";
import {SelectStory} from "./SelectStory";


export interface WriteScene {
  page: "WriteScene";
  story: LoadableP<string, Story>;
  scene: LoadableP<[string, string],Scene> | null;
  raw: string;

  isSaving: boolean;
  saveError: string | null;
  save: (state: WriteScene) => Promise<void>;
  onDelete: (state: WriteScene) => Promise<void>;
}


async function onLoad(dispatch: Dispatch, state: State) {
  var sceneName = Lens.WriteScene.scene.value.unloaded.value.get(state.page);

  let storyReq = Loadable.forRequestP<string, Story>(state,
    "GET", (story: string) => `api/v1/write/stories/${story}`,
    Lens.WriteScene.story);

  // only try to fetch the scene if we have a scene.
  let sceneReq = Loadable.forRequestP<[string,string],Scene>(
    state, "GET",
    (ss: [string,string]) => `api/v1/write/stories/${ss[0]}/scenes/${ss[1]}`,
    Lens.WriteScene.scene.value);

  let sceneRes = await sceneReq;
  let storyRes = await storyReq;

  if (storyRes.isError) {
    Dispatch.send(Dispatch.Page(SelectStory.page));
  }
  else if (sceneRes.isError) {
    // if the scene does not exist, create a new one with this name.
    if (sceneName != null && sceneRes.status == 404) {
      Dispatch.send(Dispatch.Update(
        Lens.WriteScene.raw.set("$name: " + sceneName[1] + "\n")));
      Dispatch.send(Dispatch.Update(
        Lens.WriteScene.scene.set(null)));
    }
  }

  Dispatch.send(Dispatch.Update(pages => {
    let scene = Lens.WriteScene.scene.value.loaded.result.get(pages);
    if (scene == null) {
      return null;
    }

    return Lens.WriteScene.raw.set(scene.raw)(pages);
  }));
}


async function onSave(state: WriteScene) {
  Dispatch.send(Dispatch.Update(Lens.WriteScene.isSaving.set(true)));

  let param = { text: state.raw };
  let method = state.scene === null ? "POST" as const : "PUT" as const;

  if (state.story.state != "loaded") {
    throw `Story in WriteScene state has invalid state: ${state.story.state}.`;
  }

  let story = state.story.result;
  let url = `/api/v1/write/stories/${story.urlname}/scenes`;

  if (state.scene !== null) {
    if (state.scene.state != "loaded") {
      throw `Scene in WriteScene has invalid state: ${state.scene.state}.`;
    }

    let scene = state.scene.result;
    url = url + "/" + scene.urlname;
  }

  let response = await request<Scene>(method, url, param);

  if (response.isResult) {
    Dispatch.send(Dispatch.Page(loadedPage(story, response.message)));
  }
  else {
    Dispatch.send(Dispatch.Update(Lens.WriteScene.saveError.set(response.message)));
    Dispatch.send(Dispatch.Update(Lens.WriteScene.isSaving.set(false)));
  }
}

async function onDelete(state: WriteScene) {
  if (state.scene === null) {
    throw "State.scene is null.";
  }

  if (state.story.state != "loaded" || state.scene.state != "loaded") {
    throw `Invalid state in onDelete: ${state.story.state} / ${state.scene.state}`;
  }

  let story = state.story.result;
  let response = await request<void>("DELETE",
    `/api/v1/write/stories/${story.urlname}/scenes/${state.scene.result.urlname}`);
  if (response.isError) {
    return;
  }

  Dispatch.send(Dispatch.Page(WriteStory.page(story)));
}


function instance(story: string | Story, sceneUrlname: string | null)
: WriteScene {
  let storyLoadable = typeof story == "string"
      ? Loadable.UnloadedP(story)
      : Loadable.Loaded(story);
  let storyUrlname = typeof story == "string" ? story : story.urlname;

  return {
    page: "WriteScene" as const,
    story: storyLoadable,
    scene: sceneUrlname === null
        ? null
        : Loadable.UnloadedP([storyUrlname, sceneUrlname]),
    raw: "",
    isSaving: false,
    saveError: null,
    save: onSave,
    onDelete: onDelete
  };
}


function loadedPage(story: Story, scene: Scene): PageDescriptor {
  return {
    state: {
      page: "WriteScene" as const,
      story: Loadable.Loaded(story),
      scene: Loadable.Loaded(scene),
      raw: scene.raw,
      isSaving: false,
      saveError: null,
      save: onSave,
      onDelete: onDelete
    },
    url: "/write/stories/" + story.urlname + "/scene/" + scene.urlname,
    title: "GoodNight: Szene bearbeiten",
    onLoad: onLoad
  };
}

function page(story: string | Story, sceneUrlname: string | null)
: PageDescriptor {
  let storyUrlname = typeof story == "string" ? story : story.urlname;

  return {
    state: instance(story, sceneUrlname),
    url: "/write/stories/" + storyUrlname
        + (sceneUrlname !== null ? "/scene/" + sceneUrlname : "/new-scene"),
    title: "GoodNight: "
        + (sceneUrlname !== null ? "Szene bearbeiten" : "Neue Szene"),
    onLoad: onLoad,
    requiresAuth: true
  };
}


export const WriteScene = {
  page: page,
  pageNew: (storyUrlname: string) => page(storyUrlname, null)
}

registerPageMapper(
  /^\/write\/stories\/([^\/]+)\/(scene\/([^\/]+)|new-scene)$/,
  (matches: ReadonlyArray<string>) =>
      page(matches[1], matches[3] === undefined ? null : matches[3]));

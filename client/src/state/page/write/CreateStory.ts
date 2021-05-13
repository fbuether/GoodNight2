import {Lens} from "../../Pages";
import {Dispatch} from "../../../core/Dispatch";
import type {PageDescriptor} from "../../../core/PageDescriptor";
import {request} from "../../../service/RequestService";

import type {Story} from "../../../model/write/Story";
import {WriteStory} from "./WriteStory";


export interface CreateStory {
  page: "CreateStory",
  name: string
  submit: (state: CreateStory) => Promise<void>,
  isSaving: boolean,
  saveError: string | null
}


async function submit(state: CreateStory) {
  Dispatch.send(Dispatch.Update(Lens.CreateStory.isSaving.set(true)));

  let param = { name: state.name };
  let response = await request<Story>("POST", "api/v1/write/stories", param);

  if (response.isResult) {
    let story = response.message;
    Dispatch.send(Dispatch.Page(WriteStory.page(story.urlname, story)));
  }
  else {
    Dispatch.send(Dispatch.Update(Lens.set({
      ...state,
      saveError: response.message,
      isSaving: false
    })));
  }
}


const instance = {
  page: "CreateStory" as const,
  name: "",
  submit: submit,
  isSaving: false,
  saveError: null
};

const page: PageDescriptor = {
  state: instance,
  url: "/write/new-story",
  title: "GoodNight: Eine neue Geschichte"
};

export const CreateStory = {
  path: /^\/write\/new-story$/,
  page: page,
  ofUrl: (pathname: string, matches: Array<string>) => page
};

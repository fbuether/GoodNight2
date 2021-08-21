import {Lens} from "../../Pages";
import {Dispatch} from "../../../core/Dispatch";
import {PageDescriptor, registerPageMapper} from "../../../core/PageDescriptor";
import {request} from "../../../service/RequestService";

import type {Story} from "../../model/write/Story";
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
    Dispatch.send(Dispatch.Page(WriteStory.page(story)));
  }
  else {
    Dispatch.send(Dispatch.Update(Lens.CreateStory.saveError.set(response.message)));
    Dispatch.send(Dispatch.Update(Lens.CreateStory.isSaving.set(false)));
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
  title: "GoodNight: Eine neue Geschichte",
  requiresAuth: true
};

export const CreateStory = {
  page: page
};

registerPageMapper(/^\/write\/new-story$/, page);

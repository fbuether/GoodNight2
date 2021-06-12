import {request} from "../../../service/RequestService";
import {Dispatch, DispatchAction} from "../../../core/Dispatch";
import {PageDescriptor, registerPageMapper} from "../../../core/PageDescriptor";
import type {State} from "../../State";
import {Lens} from "../../Pages";

import {UserService} from "../../../service/UserService";
import type {Story} from "../../model/read/Story";
import {Loadable} from "../../Loadable";


export interface StoryOverview {
  page: "StoryOverview";
  stories: Loadable<Array<Story>>;
  myStories: Loadable<Array<Story>>;
  onDelete: (urlname: string) => DispatchAction;
}


async function onLoad(dispatch: Dispatch, state: State) {
  await Loadable.forRequest<Array<Story>>(state,
    "GET", "api/v1/read/user/stories/mine",
    Lens.StoryOverview.myStories);

  let storiesUrl = UserService.get().getUser() != null
      ? "api/v1/read/user/stories/unread"
      : "api/v1/read/stories"
  await Loadable.forRequest<Array<Story>>(state,
    "GET", storiesUrl,
    Lens.StoryOverview.stories);
}

function onDelete(story: string) {
  return Dispatch.Command(async () => {
    let response = await request<void>("DELETE", "/api/v1/read/stories/" + story);
    if (response.isError) {
      return;
    }

    Dispatch.send(Dispatch.Page(page));
  });
}


const page: PageDescriptor = {
  state: {
    page: "StoryOverview" as const,
    stories: Loadable.Unloaded,
    myStories: Loadable.Unloaded,
    onDelete: onDelete
  },
  url: "/read",
  title: "GoodNight: Übersicht der Geschichten",
  onLoad: onLoad
};

export const StoryOverview = {
  page: page
};

registerPageMapper(/^\/read\/?$/, page);

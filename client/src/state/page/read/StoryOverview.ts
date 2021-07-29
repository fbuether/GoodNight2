import {request} from "../../../service/RequestService";
import {Dispatch} from "../../../core/Dispatch";
import {PageDescriptor, registerPageMapper} from "../../../core/PageDescriptor";
import type {State} from "../../State";
import {Pages, Lens} from "../../Pages";

import {UserService} from "../../../service/UserService";
import type {Story} from "../../model/read/Story";
import {Loadable} from "../../Loadable";


export class StoryOverview implements PageDescriptor {
  public readonly url: string = "/read";

  public readonly title: string = "GoodNight: Übersicht der Geschichten";

  public readonly state: Pages = this;

  public readonly page: "StoryOverview" = "StoryOverview" as const;

  public readonly stories: Loadable<Array<Story>> = Loadable.Unloaded;
  public readonly myStories: Loadable<Array<Story>> = Loadable.Unloaded;


  public async onLoad(dispatch: Dispatch, state: State) {
    await Loadable.forRequest<Array<Story>>(
      state, "GET", "api/v1/read/user/stories/mine",
      Lens.StoryOverview.myStories);

    let storiesUrl = UserService.get().getUser() != null
        ? "api/v1/read/user/stories/unread"
        : "api/v1/read/stories"
    await Loadable.forRequest<Array<Story>>(
      state, "GET", storiesUrl,
      Lens.StoryOverview.stories);
  }

  public onDelete(story: string) {
    return Dispatch.Command(async () => {
      let response = await request<void>("DELETE", "/api/v1/read/stories/" + story);
      if (response.isError) {
        return;
      }

      Dispatch.send(Dispatch.Page(new StoryOverview()));
    });
  }
}


registerPageMapper(/^\/read\/?$/, new StoryOverview());

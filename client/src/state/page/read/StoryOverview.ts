import request from "../../../Request";
import {Dispatch} from "../../Dispatch";
import type {State} from "../../State";
import type {PageDescriptor} from "../../model/PageDescriptor";
import {Lens} from "../../model/PageState";

import type {Story} from "../../../model/read/Story";

import {Loadable, StoryOverview as Component} from "../../../components/page/read/StoryOverview";
export type StoryOverview = Component;



async function onLoad(dispatch: Dispatch, state: State) {

  console.log("onLoad 1");

  var loadingState = Lens.StoryOverview.stories.state.get(state.page);

  if (loadingState == "unloaded") {
  console.log("onLoad 2");
    dispatch(Dispatch.Update(Lens.StoryOverview.stories.set({ state: "loading" })));

    console.log("onLoad 3");

    // await new Promise(resolve => setTimeout(resolve, 500));

    var storiesResponse = await request<Array<Story>>(
      "GET", "api/v1/read/stories");

    if (storiesResponse.isResult) {
      dispatch(Dispatch.Update(Lens.StoryOverview.stories.set({ state: "loaded", result: storiesResponse.message })));

    }
    else {
      dispatch(Dispatch.Update(Lens.StoryOverview.stories.set({ state: "failed", error: storiesResponse.message })));
    }

// p => {
//       if (p.page == "StoryOverview") {
//         return { ...p, stories: { kind: "loading" }};
//       }

//       return null;
//     }));


  }

  console.log("onLoad 4");
  // if (Lens..get(state.page) == "StoryOverview") {

  // var page = state.page;
  // if (page.page != "StoryOverview") {
  //   return;
  // }

  // if (page.stories.kind == "unloaded") {
  //   dispatch(Dispatch.Update(p => {
  //     if (p.page == "StoryOverview") {
  //       return { ...p, stories: { kind: "loading" }};
  //     }

  //     return null;
  //   }));



  //   switch (storiesResponse.kind) {
  //     case "error":
  //       console.log("request had an error.");
  //       return;
  //     case "result":
  //       console.log("request succeeded", storiesResponse);
  //       let stories = storiesResponse.message;

  //       dispatch(Dispatch.Update(p => {
  //         if (p.page == "StoryOverview") {
  //           return instance({ kind: "loaded", result: stories });
  //         }
  //         return null;
  //       }));

  //       return;
  //   }
  // }

  //   if (storiesResponse.isError) {
  //     return;
  //   }

  // }
}


function instance(stories: Loadable<Array<Story>>) {
  return {
    page: "StoryOverview" as const,
    stories: stories
  };
}

function page(stories: Loadable<Array<Story>>): PageDescriptor {
  return {
    state: instance(stories),
    url: "/read",
    title: "GoodNight: Übersicht der Geschichten",
    onLoad: onLoad,
    render: () => Component(instance(stories))
  };
}

export const StoryOverview = {
  path: /^\/read$/,
  page: page({ state: "unloaded" }),
  ofUrl: (pathname: string, matches: Array<string>) => page({ state: "unloaded" })
};

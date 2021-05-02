import type {Story} from "../../model/read/Story";

import Loading from "../../components/common/Loading";


export type Unloaded = { state: "unloaded"; }
export type Loading = { state: "loading"; }
export type Loaded<T> = { state: "loaded"; result: T; }
export type Failed = { state: "failed"; error: string; }

export type Loadable<T> = Unloaded | Loading | Loaded<T> | Failed;





export interface StoryOverview {
  page: "StoryOverview";
  stories: Loadable<Array<Story>>;
}


export function StoryOverview(state: StoryOverview) {
  let stories = state.stories;

  switch (state.stories.state) {
    case "loaded":
      return (<div>Story Overview! {state.stories.result.length}</div>);
    case "failed":
      return <div>Could not load stories: {state.stories.error}</div>;
    default:
      return <Loading />;
  }
}



import type {Story} from "../../../model/read/Story";


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
  let count = stories.state == "loaded" ? stories.result.length : stories.state;

  return (<div>Story Overview! {count}</div>);
}


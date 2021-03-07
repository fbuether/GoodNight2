import {State} from "../state/State";
import request from "../Request";

import {Adventure, asAdventure} from "../model/read/Adventure";


export async function loadStory(user: string, story: string)
: Promise<Adventure> {
  let response = await request("GET", "api/v1/read/continue");
  return asAdventure(response);
}

export function showStory(state: State, adventure: Adventure): State {
  return {
    ...state,
    page: {
      kind: "read" as const,
      story: adventure.story.name,
      user: adventure.player.name,
      adventure: adventure
    }
  };
}

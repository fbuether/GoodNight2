import type State from "../model/State";
import request from "../Request";

// import {Adventure, asAdventure} from "../model/read/Adventure";
import {Option} from "../model/read/Scene";

// export async function loadStory(user: string, story: string)
// : Promise<Adventure> {
//   let response = await request("api/v1/read/continue");
//   return asAdventure(response);
// }

export function readOption(state: State, option: Option): State {
  console.log("reading option.");

  return {
    ...state// ,
    // page: {
    //   kind: "read" as const,
    //   story: adventure.story.name,
    //   user: adventure.player.name,
    //   adventure: adventure
    // }
  };
}

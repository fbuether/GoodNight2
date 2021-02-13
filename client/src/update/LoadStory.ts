import type State from "../model/State";
import request from "../Request";

import type {Player} from "../model/read/Player";


export async function loadStory(user: string, story: string): Promise<Player> {
  console.log("loading story");
  let player = await request("api/v1/read/continue") as Player;
  return player;
}

export function showStory(state: State, player: Player): State {
  console.log("showin story");
  return {
    ...state,
    page: {
      kind: "read" as const,
      story: player.story,
      user: player.user,
      player: player
    }
  };
}


// export default async function loadStory(user: string, story: string) {

//   return (state: State) => {
//     console.log("loading story. right?");

//     return { ...state, page: { ...state.page, player: player } };
//   };
// }

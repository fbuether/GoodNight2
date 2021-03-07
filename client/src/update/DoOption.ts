import {State} from "../state/State";
import request from "../Request";

import {Adventure} from "../model/read/Adventure";
import type {Option} from "../model/read/Scene";
import {Consequence, asConsequence} from "../model/read/Consequence";


export async function doOption(option: Option): Promise<Consequence> {
  // todo: add story to url. .../api/v1/read/{story}/do
  return await request("POST", "api/v1/read/do", option.scene) as Consequence;
}



// export async function loadStory(user: string, story: string)
// : Promise<Adventure> {
//   let response = await request("api/v1/read/continue");
//   return asAdventure(response);
// }

export function showOption(state: State, consequence: Consequence): State {
  console.log("reading option.", consequence);

  let currPage = state.page; // as ReadPage;

  if (currPage.kind == "read") {
  //   console.error("showOption in non-read state!");
  // }

    let adv = currPage.adventure as Adventure;
    if (currPage.adventure != undefined) {
    let hist = currPage.adventure.history;

  return {
    ...state,
    page: {
      ...currPage,
      adventure: {
        ...adv,
        history: hist.concat([consequence.action]),
        current: consequence.scene
      }
    }
  };
}
    else {
      throw "blurg";
    }
  }
  else {
    throw "blarg";
  }
}


// ,
    // page: {
    //   kind: "read" as const,
    //   story: adventure.story.name,
    //   user: adventure.player.name,
    //   adventure: adventure
    // }
  // };
// }

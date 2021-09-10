import type {Test} from "../../../state/model/read/Action";

import {makeIcon} from "../../common/Icon";


export default function Test(state: Test) {
  let colour = 5 - Math.floor(state.chance / 20);
  let colClass = "ampel-" + colour + "5";
  let icon = makeIcon(state.icon, "mr-1");

  return (
    <span class={colClass}>{icon}{state.display} ({state.chance}%)</span>
  );
}

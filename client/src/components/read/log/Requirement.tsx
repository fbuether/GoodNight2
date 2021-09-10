import type {Requirement} from "../../../state/model/read/Action";

import {makeIcon} from "../../common/Icon";


export default function Requirement(state: Requirement) {
  let passedClass = state.passed ? "" : "ampel-55";
  let icon = makeIcon(state.icon, "mr-1");

  // todo: fixme: fix requirement display. {state.expression}
  return (
    <span class={passedClass}>{icon}{state.display}</span>
  );
}

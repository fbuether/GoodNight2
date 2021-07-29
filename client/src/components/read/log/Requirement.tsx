import type {Requirement} from "../../../state/model/read/Action";

import Icon from "../../common/Icon";


export default function Requirement(state: Requirement) {
  let passedClass = state.passed ? "" : "text-danger";

  // todo: fixme: fix requirement display. {state.expression}
  return (
    <span class={passedClass}>{state.display}</span>
  );
}

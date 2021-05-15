import type {Requirement} from "../../model/read/Action";

import Icon from "../common/Icon";


export default function Requirement(state: Requirement) {
  let passedClass = state.passed ? "" : "warning";

  // todo: fixme: fix requirement display. {state.expression}
  return (
    <span class="{passedClass}">Requirement</span>
  );
}

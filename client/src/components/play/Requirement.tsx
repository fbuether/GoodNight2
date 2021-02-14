import Icon, {IconName} from "../play/Icon";

import type {Requirement} from "../../model/read/Scene";


// export default interface Requirement {
//   name: string,
//   icon: IconName,
//   required: string,
//   relation: string,
//   has: string
// }

export default function Requirement(state: Requirement) {
  let passedClass = state.passed ? "" : "warning";

  return (
    <span class="{passedClass}">{state.description}</span>
  );
}

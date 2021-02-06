import * as React from "react";
import Icon, {Icons} from "../play/Icon";

export default interface Requirement {
  name: string,
  icon: typeof keyof Icons,
  required: string,
  relation: string,
  has: string
}

export default function Requirement(state: Requirement) {

  return (
    <span key={state.name}>
      {state.relation} {state.required}/{state.has}
      <Icon name={state.icon} /> {state.name}
    </span>
  );
}

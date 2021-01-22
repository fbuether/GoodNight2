import * as React from "react";

export default interface Requirement {
  name: string,
  required: string,
  relation: string,
  has: string
}

export default function Requirement(state: Requirement) {
  return (
    <span>{state.relation} {state.required}/{state.has} {state.name}</span>
  );
}

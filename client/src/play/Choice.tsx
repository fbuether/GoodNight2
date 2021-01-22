import * as React from "react";
import Requirement from "../play/Requirement";


export default interface Choice {
  name: string;
  text: string;
  available: boolean;
  requirements: Array<Requirement>;
}

export default function Choice(choice: Choice) {
  let hasReqs = choice.requirements.length > 0;

  if (!hasReqs) {
    return (
      <a key={choice.name} className="list-group-item list-group-item-action" href="#">
        {choice.text}
      </a>
    );
  }

  let addReq = (rs: Array<JSX.Element>, r: Requirement) => {
    if (rs.length > 0) {
      rs.push(<span>, </span>);
    }
    rs.push(<Requirement key={r.name} {...r}></Requirement>);
    return rs;
  };

  let requirements = choice.requirements.reduce(addReq,
    new Array<JSX.Element>());

  if (!choice.available) {
    return (
      <div key={choice.name} className="list-group-item list-group-item-action disabled">
        {choice.text} {" "}
        <span className="font-size-sm">
          (nicht verfügbar. benötigt: {requirements})
        </span>
      </div>
    );
  }

  return (
    <a key={choice.name} className="list-group-item list-group-item-action" href="#">
      {choice.text} {" "}
      <span className="font-size-sm text-secondary">
        (benötigt: {requirements})
      </span>
    </a>
  );
}

import Link from "../../components/common/Link";
import type {Page} from "../../model/Page";

import RequirementC from "../play/Requirement";

import type {Option, Requirement} from "../../model/read/Scene";

// export default interface Option {
//   name: string;
//   text: string;
//   available: boolean;
//   requirements: Array<Requirement>;
// }

export default function Option(option: Option) {
  let hasReqs = option.requirements.length > 0;

  if (!hasReqs) {
    if (option.isAvailable) {
          // to={{kind: "ReadAction"}}
      return (
        <div class="list-group-item list-group-item-action"
            >
          {option.text}
        </div>
      );
    }
    else {
      return (
        <a class="list-group-item list-group-item-action disabled">
          {option.text} <small>(nicht verfügbar.)</small>
        </a>
      );
    }
  }

  let addReq = (rs: Array<JSX.Element>, r: Requirement, index: number) => {
    if (rs.length > 0) {
      rs.push(<span key={"empty"}>, </span>);
    }
    rs.push(<RequirementC key={index} {...r}></RequirementC>);
    return rs;
  };

  let requirements = option.requirements.reduce(addReq,
    new Array<JSX.Element>());

  if (!option.isAvailable) {
    return (
      <a class="list-group-item list-group-item-action disabled">
        {option.text}{" "}
        <small>(nicht verfügbar. benötigt: {requirements})</small>
      </a>
    );
  }
  else {
    return (
      <div class="list-group-item list-group-item-action">
        {option.text} <small>(benötigt: {requirements})</small>
      </div>
    );
  }
}

import Link from "../../components/common/Link";
import type {Page} from "../../model/Page";

import RequirementC from "../play/Requirement";

import type {Option, Requirement} from "../../model/read/Scene";


export default function Option(option: Option) {
  let hasReqs = option.requirements.length > 0;

  if (!hasReqs) {
    if (option.isAvailable) {
          // to={{kind: "ReadAction"}}
      return (
        <button class="list-group-item list-group-item-action">
          {option.text}
        </button>
      );
    }
    else {
      return (
        <button class="list-group-item list-group-item-action" disabled>
          {option.text} <small>(nicht verfügbar.)</small>
        </button>
      );
    }
  }

  let addReq = (rs: Array<JSX.Element>, r: Requirement, index: number) => {
    if (rs.length > 0) {
      rs.push(<span>, </span>);
    }
    rs.push(<RequirementC {...r}></RequirementC>);
    return rs;
  };

  let requirements = option.requirements.reduce(addReq,
    new Array<JSX.Element>());

  if (!option.isAvailable) {
    return (
      <button class="list-group-item list-group-item-action" disabled>
        {option.text}{" "}
        <small>(nicht verfügbar. benötigt: {requirements})</small>
      </button>
    );
  }
  else {
    return (
      <button class="list-group-item list-group-item-action">
        {option.text} <small>(benötigt: {requirements})</small>
      </button>
    );
  }
}

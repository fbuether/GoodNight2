import type {Option} from "../../model/read/Action";

import Link from "../common/Link";
import Requirement from "./Requirement";


export default function Option(option: Option) {
  // const dispatch = PreactHooks.useContext(DispatchContext);

  let requirements: JSX.Element | string = "";

  if (option.requirements.length > 0) {
    let requirementList = <>{option.requirements.map(Requirement)}</>;
    let available = option.isAvailable ? "" : "nicht verfügbar. ";
    requirements = <small>({available}benötigt: {requirementList})</small>;
  }
  else {
    if (!option.isAvailable) {
      requirements = <small>(nicht verfügbar.)</small>;
    }
  }

  if (option.isAvailable) {
    // todo: onClick={dispatchOption(dispatch, option)}
    return (
      <button class="list-group-item list-group-item-action">
        {option.text} {requirements}
      </button>
    );
  }
  else {
    return (
      <button class="list-group-item list-group-item-action" disabled>
        {option.text} {requirements} 
      </button>
    );
  }
}

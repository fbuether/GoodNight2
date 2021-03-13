import * as PreactHooks from "preact/hooks";

import Link from "../../components/common/Link";
import RequirementC from "../../components/play/Requirement";

import DispatchContext from "../../DispatchContext";
import {Update} from "../../state/State";

import type {Page} from "../../state/Page";
import type {Option, Requirement} from "../../model/read/Scene";

// import {doOption} from "../../update/DoOption";

function dispatchOption(dispatch: (u: Update) => void, option: Option) {
  return async (event: MouseEvent) => {
    event.preventDefault();
    // let consequence = await doOption(option);
    // dispatch({ kind: "ReadOption", consequence: consequence });
  }
}


export default function Option(option: Option) {
  const dispatch = PreactHooks.useContext(DispatchContext);

  let requirements: JSX.Element | string = "";

  if (option.requirements.length > 0) {
    let requirementList = (
      <>
        {option.requirements.map(r => <RequirementC {...r} />)}
      </>
    );

    requirements = option.isAvailable
        ? <small>(benötigt: {requirementList})</small>
        : <small>(nicht verfügbar. benötigt: {requirementList})</small>;
  }
  else {
    if (!option.isAvailable) {
      requirements = <small>(nicht verfügbar.)</small>;
    }
  }

  if (option.isAvailable) {
    return (
      <button class="list-group-item list-group-item-action"
        onClick={dispatchOption(dispatch, option)}>
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

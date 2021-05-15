import type {OptionState} from "../../state/page/read/ReadStory";

import Link from "../common/Link";
import Markdown from "../common/Markdown";
import Requirement from "./Requirement";



export default function Option(state: OptionState) {
  var doOption = (event: MouseEvent) => {
    event.preventDefault();
    state.onOption(state.option.urlname, state.option.text);
  }

  let option = state.option;

  let requirements;
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
    return (
      <button class="list-group-item list-group-item-action" onClick={doOption}>
        <Markdown>{option.text}</Markdown>
        {requirements}
      </button>
    );
  }
  else {
    return (
      <button class="list-group-item list-group-item-action" disabled>
        <Markdown>{option.text}</Markdown>
        {requirements}
      </button>
    );
  }
}

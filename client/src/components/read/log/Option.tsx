import type {OptionState} from "../../../state/page/read/ReadStory";
import type {Option} from "../../../state/model/read/Action";

import Link from "../../common/Link";
import Markdown from "../../common/Markdown";
import Requirement from "./Requirement";


function Requirements(option: Option) {
  if (option.requirements.length <= 0 && option.isAvailable) {
    return <></>;
  }

  var avail = option.isAvailable ? "" : "nicht verfügbar.";
  var requires = option.requirements.length > 0
      ? (!option.isAvailable ? " " : "") + "benötigt: "
      : "";

  var requirements = option.requirements.map(Requirement);
  for (let i = requirements.length; i = i - 1; i > 0) {
    requirements.splice(i, 0, <>; </>);
  }

  return (
    <small class="float-end mw-50 px-2 pt-1">
      (<em>{avail}{requires}</em>{requirements})
    </small>
  );
}


export default function Option(state: OptionState) {
  var doOption = (event: MouseEvent) => {
    event.preventDefault();
    state.onOption(state.option.urlname, state.option.text);
  }

  let option = state.option;

  let classes = "list-group-item list-group-item-action";
  let content = <>{Requirements(option)}<Markdown>{option.text}</Markdown></>;

  return option.isAvailable
      ? <button class={classes} onClick={doOption}>{content}</button>
      : <button class={classes} disabled>{content}</button>;
}

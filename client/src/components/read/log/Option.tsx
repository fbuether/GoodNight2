import type {OptionState} from "../../../state/page/read/ReadStory";
import type {Option, Test as TestT} from "../../../state/model/read/Action";

import Link from "../../common/Link";
import Markdown from "../../common/Markdown";
import Test from "./Test";
import Requirement from "./Requirement";


function Requirements(option: Option) {
  if (option.isAvailable
      && option.tests.length == 0
      && option.requirements.length == 0) {
    return <></>;
  }

  let available = !option.isAvailable
      ? <small><em>nicht verfügbar.</em></small>
      : <></>;
  let tests = option.tests.length > 0
      ? <small><em>prüft:</em> {option.tests.map(Test)}</small>
      : <></>;
  let requirements = option.requirements.length > 0
      ? <small><em>benötigt:</em> {option.requirements.map(Requirement)}</small>
      : <></>;
  return (
    <div class="requirements float-end mw-50 px-2 py-1 text-end">
      {available}{tests}{requirements}
    </div>
  );
}


export default function Option(state: OptionState) {
  let doOption = (event: MouseEvent) => {
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

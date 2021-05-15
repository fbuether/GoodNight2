import type {ActionState} from "../../state/page/read/ReadStory";
import type {Action as State} from "../../model/read/Action";

import Markdown from "../../components/common/Markdown";
import Effect from "../../components/read/Effect";
import Option from "../../components/read/Option";




export default function Action(state: ActionState) {
  // todo: return, continue.
  let action = state.action;

  return (
    <>
      <Markdown>{action.text}</Markdown>
      {action.effects.map(Effect)}
      <hr class="w-75 mx-auto mt-4 mb-5" />
      <div class="options list-group">
        {action.options.map(opt => Option({ option: opt, onOption: state.onOption }))}
      </div>
    </>
  );

}

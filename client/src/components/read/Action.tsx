import type {Action as State} from "../../model/read/Action";

import Markdown from "../../components/common/Markdown";
import Effect from "../../components/read/Effect";
import Option from "../../components/read/Option";


export default function Action(action: State) {
  // todo: return, continue.

  return (
    <>
      <Markdown>{action.text}</Markdown>
      {action.effects.map(Effect)}
      <hr class="w-75 mx-auto mt-4 mb-5" />
      <div class="options list-group">
        {action.options.map(Option)}
      </div>
    </>
  );

}

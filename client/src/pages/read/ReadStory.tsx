import type {Adventure} from "../../model/read/Adventure";
import type {Story} from "../../model/read/Story";
import type {Option as OptionState} from "../../model/read/Action";
import type {ReadStory as State} from "../../state/page/read/ReadStory";

import Error from "../../components/common/Error";
import Icon from "../../components/common/Icon";
import LoadableLoader from "../../components/common/LoadableLoader";
import Markdown from "../../components/common/Markdown";

import Log from "../../components/read/Log";
import Player from "../../components/read/Player";
import Effect from "../../components/read/Effect";
import Option from "../../components/read/Option";



function Options(options: Array<OptionState>,
  onOption: (option: OptionState) => Promise<void>) {
  let optionArgs = options.map(opt => ({
    option: opt,
    onOption: onOption
  }));

  return (
    <div class="options list-group">
      {optionArgs.map(Option)}
    </div>
  );
}


function LoadingChoice(choice: OptionState) {
  return (
    <div class="options list-group">
      <div class="list-group-item choice">
        <Icon name="empty-hourglass" class="mr-2 float-end" />
        {choice.text}
      </div>
    </div>
  );
}


function ReadStoryLoaded(state: State, story: Story, adventure: Adventure) {
  let onOption = (option: OptionState) => state.onOption(state, option);
  let action = adventure.current;

  let options = state.choice === null
      ? Options(action.options, onOption)
      : LoadingChoice(state.choice);

  return (
    <div id="centre" class="row px-0 g-0">
      <div id="text" class="col-sm-8">
        <h1 id="banner">{story.name}</h1>
        <Log entries={adventure.history}></Log>
        <hr class="w-75 mx-auto mt-4 mb-5" />
        <Markdown>{action.text}</Markdown>
        {action.effects.map(Effect)}
        <Error message={state.error} />
        {options}
      </div>
      <div id="side" class="col-sm-4">
        <hr class="w-75 mx-auto mt-4 mb-5" />
        <Player {...adventure.player} />
      </div>
    </div>
  );
}

export function ReadStory(state: State) {
  return LoadableLoader(state.adventure, adventure =>
      LoadableLoader(state.story, story =>
          ReadStoryLoaded(state, story, adventure)));
}

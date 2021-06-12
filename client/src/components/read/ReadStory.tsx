import type {Adventure} from "../../state/model/read/Adventure";
import type {Story} from "../../state/model/read/Story";
import type {Option as OptionState} from "../../state/model/read/Action";
import type {ReadStory as State} from "../../state/page/read/ReadStory";

import Error from "../../components/common/Error";
import Icon from "../../components/common/Icon";
import LoadableLoader from "../../components/common/LoadableLoader";
import Markdown from "../../components/common/Markdown";

import Log from "./log/Log";
import Player from "./log/Player";
import Effect from "./log/Effect";
import Option from "./log/Option";



function Options(options: Array<OptionState>,
  onOption: (urlname: string, choice: string) => Promise<void>) {
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


function LoadingChoice(choice: string) {
  return (
    <div class="options list-group">
      <div class="list-group-item choice">
        <Icon name="empty-hourglass" class="mr-2 float-end" />
        <Markdown>{choice}</Markdown>
      </div>
    </div>
  );
}


function ReadStoryLoaded(state: State, story: Story, adventure: Adventure) {
  let onOption = (urlname: string, choice: string) =>
      state.onOption(state, urlname, choice);
  let doOption = (target: string) => (event: MouseEvent) => {
    event.preventDefault();
    onOption(target, target == "return" ? "Zurück…" : "Weiter…");
  };


  let action = adventure.current;
  let options = state.choice === null
      ? Options(action.options, onOption)
      : LoadingChoice(state.choice);

  let returnLink = action.hasReturn
      ? <a class="boxed clickable" onClick={doOption("return")}>← Zurück</a>
      : <div />;
  let continueLink = action.hasContinue
      ? <a class="boxed clickable" onClick={doOption("continue")}>Weiter →</a>
      : <div />;

  return (
    <div id="centre" class="row px-0 g-0">
      <div id="text" class="col-sm-8">
        <h1 id="banner">{story.name}</h1>
        <Log entries={adventure.history}></Log>
        <Markdown>{action.text}</Markdown>
        {action.effects.map(Effect)}
        <Error message={state.error} />
        {options}
        <div class="d-flex w-75 mx-auto mt-3 justify-content-between align-items-center">
          {returnLink}
          {continueLink}
        </div>
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

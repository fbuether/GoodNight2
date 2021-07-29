import {JSX} from "preact";
import {Dispatch} from "../../core/Dispatch";
import {Lens} from "../../state/Pages";

import type {StartAdventure as State} from "../../state/page/read/StartAdventure";
import {StoryOverview} from "../../state/page/read/StoryOverview";

import type {Story} from "../../state/model/read/Story";

import Icon from "../../components/common/Icon";
import Link from "../../components/common/Link";
import Error from "../../components/common/Error";
import SaveButton from "../../components/common/SaveButton";
import LoadableLoader from "../../components/common/LoadableLoader";


function StartAdventureInner(state: State, story: Story) {
  let returnLink = Dispatch.Page(new StoryOverview());
  let submit = (event: Event) => {
    event.preventDefault();
    state.onStart(state);
  };

  let setName = (event: JSX.TargetedEvent<HTMLInputElement, Event>) => {
    let newName = event.currentTarget.value;
    Dispatch.send(Dispatch.Update(Lens.StartAdventure.name.set(newName)));
  };

  return (
    <div id="centre" class="row px-0 g-0">
      <h1>{story.name}</h1>
      <p>{story.description}</p>
      <form class="my-3 w-75 mx-auto" onSubmit={submit}>
        <label for="playerName" class="form-label">
          Wähle einen Namen für Deinen Charakter:
        </label>
        <input id="playerName" type="text" class="form-control"
          onInput={setName} />

        <div class="d-flex w-75 mx-auto mt-3 justify-content-between align-items-center">
          <Link action={returnLink}>Zurück</Link>
          <SaveButton isSaving={state.isStarting} />
        </div>
      </form>
    </div>
  );
}


export function StartAdventure(state: State) {
  return LoadableLoader(state.story, story =>
      StartAdventureInner(state, story));
}

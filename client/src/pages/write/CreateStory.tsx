import {JSX} from "preact";
import {Dispatch} from "../../core/Dispatch";
import {Lens} from "../../state/Pages";

import type {CreateStory as State} from "../../state/page/write/CreateStory";
import {SelectStory} from "../../state/page/write/SelectStory";

import Link from "../../components/common/Link";
import Error from "../../components/common/Error";
import SaveButton from "../../components/common/SaveButton";


export function CreateStory(state: State) {
  let returnLink = Dispatch.Page(SelectStory.page);
  let submit = (event: Event) => {
    event.preventDefault();
    state.submit(state);
  };

  let setName = (event: JSX.TargetedEvent<HTMLInputElement, Event>) => {
    let newName = event.currentTarget.value;
    Dispatch.send(Dispatch.Update(Lens.CreateStory.name.set(newName)));
  };

  return (
    <div id="centre" class="row px-0 g-0">
      <h1>Eine neue Geschichte</h1>
      <p>
        Eine neue Geschichte beginnt stets mit einem Namen. Dieser Name kann
        später geändert werden, allerdings nicht seine URL-Form.
      </p>
      <form class="my-3 w-75 mx-auto" onSubmit={submit}>
        <label for="nameOfStory" class="form-label">
          Der Name der neuen Geschichte
        </label>
        <input id="nameOfStory" type="text" class="form-control"
          onChange={setName} />

        <Error message={state.saveError} />
        <div class="d-flex w-75 mx-auto mt-3 justify-content-between align-items-center">
          <Link action={returnLink}>Zurück</Link>
          <SaveButton isSaving={state.isSaving} />
        </div>
      </form>
    </div>
  );
}

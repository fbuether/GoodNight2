import * as PreactHooks from "preact/hooks";
import DispatchContext from "../../DispatchContext";
import useAsyncEffect from "../../ui/useAsyncEffect";

import {Story} from "../../model/write/Story";
import {State, Dispatch} from "../../state/State";
import {CreateStoryPart} from "../../state/write/CreateStoryPart";

import Link from "../common/Link";
import Loading from "../common/Loading";




export default function CreateStory(state: CreateStoryPart) {
  return (
    <div id="centre" class="row px-0 g-0">
      <h1>Eine neue Geschichte</h1>
      <p>
        Wähle eine Geschichte, die du schreiben willst, oder erstelle eine
        neue Geschichte.
      </p>
    </div>
  );
}

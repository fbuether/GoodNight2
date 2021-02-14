import * as PreactHooks from "preact/hooks";

import DispatchContext from "../DispatchContext";
import useAsyncEffect from "../ui/useAsyncEffect";

import type {ReadPage} from "../model/Page";
import {loadStory} from "../update/LoadStory";

import Scene from "../components/play/Scene";
import State from "../components/play/State";
import Log from "../components/play/Log";



export default function Read(page: ReadPage) {
  const dispatch = PreactHooks.useContext(DispatchContext);

  if (page.adventure == null) {
    useAsyncEffect(async() => {
      let adventure = await loadStory(page.user, page.story);
      dispatch({ kind: "ShowStory", adventure: adventure });
    });

    return (
      <div id="centre" class="row px-0 g-0">
        <div id="text" class="col-sm-8">
          <h1 id="banner">Loading…</h1>
        </div>
        <div id="side" class="col-sm-4">
        </div>
      </div>
    );
  }
  else {
    let adventure = page.adventure;

    return (
      <div id="centre" class="row px-0 g-0">
        <div id="text" class="col-sm-8">
          <h1 id="banner">{adventure.story.name}</h1>
          <Log entries={adventure.history}></Log>
          <Scene {...adventure.current}></Scene>
        </div>
        <div id="side" class="col-sm-4">
          <hr class="w-75 mx-auto mt-4 mb-5" />
          <State {...adventure.player}></State>
        </div>
      </div>
    );
  }
}

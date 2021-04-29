import * as PreactHooks from "preact/hooks";
import DispatchContext from "../../DispatchContext";
import useAsyncEffect from "../../ui/useAsyncEffect";
import request from "../../Request";

import type {Adventure} from "../../model/read/Adventure";

import {State, Dispatch} from "../../state/State";
import {ReadStoryPart} from "../../state/read/ReadStoryPart";


import Link from "../common/Link";
import Icon from "../common/Icon";
import Loading from "../common/Loading";


function loadAdventure(dispatch: Dispatch, story: string) {
  return async () => {
    let response = await request<Adventure>(
      "GET", `api/v1/read/stories/${story}/continue`);


    // todo!

    // if (response.isError) {

    // }

    
  };
}


export default function ReadStory(state: ReadStoryPart) {
  const dispatch = PreactHooks.useContext(DispatchContext);

  if (state.adventure == null) {
    useAsyncEffect(loadAdventure(dispatch, state.story));
    return <Loading />;
  }

  return (
    <div id="centre" class="row px-0 g-0">
      <div id="text" class="col-sm-8">
        ~data~
      </div>
    </div>
  );

      //   <h1 id="banner">{adventure.story.name}</h1>
      //   <Log entries={adventure.history}></Log>
      //   <Scene {...adventure.current}></Scene>
      // </div>
      // <div id="side" class="col-sm-4">
      //   <hr class="w-75 mx-auto mt-4 mb-5" />
      //   <State {...adventure.player}></State>

}

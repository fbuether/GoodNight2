import * as PreactHooks from "preact/hooks";

import DispatchContext from "../../DispatchContext";
import useAsyncEffect from "../../ui/useAsyncEffect";

import type {ReadPage} from "../../model/Page";
import {loadStory} from "../../update/LoadStory";

import Frame from "../play/Frame";



export default function Read(state: { page: ReadPage }) {
  const dispatch = PreactHooks.useContext(DispatchContext);

  if (state.page.player == null) {
    useAsyncEffect(async() => {
      dispatch({
        kind: "ShowStory",
        player: await loadStory(state.page.user, state.page.story) });
    });

    return (
      <div>Loading…</div>
    );
  }
  else {
    return (
      <Frame />
    );
  }
}

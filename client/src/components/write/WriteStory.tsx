import * as PreactHooks from "preact/hooks";
import DispatchContext from "../../DispatchContext";
import useAsyncEffect from "../../ui/useAsyncEffect";
import request from "../../Request";

import {State, Dispatch} from "../../state/State";
import {Story} from "../../model/write/Story";

import {SelectStoryPart} from "../../state/write/SelectStoryPart";
import {WriteStoryPart, WritePart} from "../../state/write/WriteStoryPart";
import {StoryOverview as StoryOverviewState}
  from "../../state/write/StoryOverview";

import Link from "../common/Link";
import Icon from "../common/Icon";
import Loading from "../common/Loading";

import StoryOverview from "./StoryOverview";
import WriteScene from "./WriteScene";


function loadStory(dispatch: Dispatch, state: WriteStoryPart, name: string) {
  return async () => {
    let response = await request<Story>("GET", `api/v1/write/stories/${name}`);
    if (response.isError) {
      return;
    }

    dispatch(State.lens.page.write.part.writeStory.story.set(response.message));
  };
}

function assertNever(param: never): never {
  throw new Error(`Invalid Page kind in state WriteStory: "${param}"`);
}

function getPage(page: WritePart) {
  switch (page.kind) {
    case "StoryOverview": return <StoryOverview {...page} />;
    case "WriteScene": return <WriteScene {...page} />;
    default: return assertNever(page);
  }
}

export default function WriteStory(state: WriteStoryPart) {
  const dispatch = PreactHooks.useContext(DispatchContext);

  if (typeof state.story == "string") {
    useAsyncEffect(loadStory(dispatch, state, state.story));
    return (
      <div id="centre">
        <Loading class="mt-4" />
      </div>
    );
  }

  let urlname = typeof state.story == "string"
      ? state.story
      : state.story.urlname;

  let toBase = State.lens.page.write.part.writeStory.part
    .set(StoryOverviewState.instance(urlname));

  return (
    <div id="centre" class="px-0">
      <h1><Link target={toBase}>Schreibe: {state.story.name}</Link></h1>

      {getPage(state.part)}
    </div>
  );
}

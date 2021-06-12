import {Dispatch} from "../../core/Dispatch";
import {Lens} from "../../state/Pages";
import {WriteScene as State} from "../../state/page/write/WriteScene";
import {WriteStory} from "../../state/page/write/WriteStory";

import type {Scene} from "../../model/write/Scene";
import type {Story} from "../../model/write/Story";

import SceneHelp from "../../components/write/SceneHelp";

import Icon from "../../components/common/Icon";
import Link from "../../components/common/Link";
import Error from "../../components/common/Error";
import SaveButton from "../../components/common/SaveButton";
import LoadableLoader from "../../components/common/LoadableLoader";
import ScalingTextarea from "../../components/common/ScalingTextarea";


function setText(newText: string) {
  return Dispatch.Update(Lens.WriteScene.raw.set(newText));
}


function mkLinkGroup(story: string, title: string, links: Array<string>) {
  if (links.length == 0) {
    return <></>;
  }

  let sceneLink = (link: string) => Dispatch.Page(State.page(story, link));

  let linkItems = links.map(link => (<li>
      <Link action={sceneLink(link)}>
        <Icon class="restrained color-primary mr-1" name="horizon-road" />
        {link}
      </Link>
    </li>));
  return <>
    <h5>{title}:</h5>
    <ul>{linkItems}</ul>
  </>;
}


export function WriteSceneLoaded(state: State, story: Story, scene: Scene | null) {
  let returnLink = Dispatch.Page(WriteStory.page(story.urlname, story));
  let submit = (event: Event) => {
    event.preventDefault();
    state.save(state);
  };

  let doDelete = (event: Event) => {
    event.preventDefault();
    state.onDelete(state);
  }

  let links = <></>;
  if (scene !== null && scene.outLinks !== null && scene.inLinks !== null) {
    links = <>
      {mkLinkGroup(story.urlname, "Ausgehende Links", scene.outLinks)}
      {mkLinkGroup(story.urlname, "Eingehende Links", scene.inLinks)}
    </>;
  }

  return (
    <div id="centre" class="px-0">
      <h1><Link action={returnLink}>Schreibe: {story.name}</Link></h1>

      <div class="row">
        <form class="col-8" onSubmit={submit}>
          <h2>{scene === null ? "Neue Szene" : scene.name}</h2>

          <ScalingTextarea class="form-control larger"
            onChange={setText}
            content={state.raw} />

          <Error message={state.saveError} />
          <div class="my-3 mx-4 d-flex justify-content-end g-4">
            <button class="btn btn-danger mx-3"
              onClick={doDelete}>
              <Icon name="tombstone" />
              Löschen
            </button>
            <SaveButton isSaving={state.isSaving} />
          </div>

          <div class="flex-grow-1">
            {links}
          </div>
        </form>
        <div class="col-4">
          <SceneHelp />
        </div>
      </div>
    </div>
  );
}


export function WriteScene(state: State) {
  return LoadableLoader(state.story, story => {
    if (state.scene !== null) {
      return LoadableLoader(state.scene, scene =>
          WriteSceneLoaded(state, story, scene));
    }
    else {
      return WriteSceneLoaded(state, story, null);
    }
  });
}

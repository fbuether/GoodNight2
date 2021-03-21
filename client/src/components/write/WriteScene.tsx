import {JSX} from "preact";
import request from "../../Request";
import * as PreactHooks from "preact/hooks";

import DispatchContext from "../../DispatchContext";
import useAsyncEffect from "../../ui/useAsyncEffect";

import {Scene} from "../../model/write/Scene";

import {State, Dispatch} from "../../state/State";
import {WriteScene as WriteSceneState} from "../../state/write/WriteScene";

import Loading from "../common/Loading";


function submit(dispatch: Dispatch, state: WriteSceneState) {
  return async(event: JSX.TargetedEvent<HTMLFormElement, Event>) => {
    event.preventDefault();

    let param = {
      text: state.scene
    };

    console.log("state:", state);

    let response = state.urlname === null
        ? await request<Scene>("POST", `/api/v1/write/story/${state.story}/scenes`, param)
        : await request<Scene>("PUT", `/api/v1/write/story/${state.story}/scenes/${state.urlname}`, param);

    if (response.isError) {
      return;
    }

    dispatch(State.lens.page.write.part.writeStory.part.writeScene.set({
      kind: "WriteScene" as const,
      scene: response.message.raw,
      story: state.story,
      urlname: response.message.urlname
    }));
  };
}



function setText(dispatch: Dispatch) {
  return (event: JSX.TargetedEvent<HTMLTextAreaElement, Event>) => {
    dispatch(State.lens.page.write.part.writeStory.part.writeScene.scene
      .set(event.currentTarget.value));
  };
}



export default function WriteScene(state: WriteSceneState) {
  const dispatch = PreactHooks.useContext(DispatchContext);

  let title = state.urlname === null ? "Neue Szene" : "Szene bearbeiten";

  console.log(state);

  if (state.urlname !== null && state.scene === null) {
    // todo: request this single scene, store its raw text in state.scene.

    return <Loading />;
  }


  return (
    <div class="row">

      <form class="col-8" onSubmit={submit(dispatch, state)}>
        <h2>{title}</h2>

        <textarea class="form-control"
          onChange={setText(dispatch)}>{state.scene}
        </textarea>

        <div class="buttons w-75 mx-auto mt-3 text-end">
          <button type="submit" class="btn btn-primary">
            Speichern
          </button>
        </div>

      </form>

      <div class="col-4">
        <h2>Referenz</h2>
        <p>
          Eine Szene kann verschiedene Platzhalter enthalten, um ihr Verhalten
          zu steuern.
        </p>
        <dl class="commands">
          <dt>$name: <em>name</em></dt>
          <dd>
            Legt den Namen dieser Szene fest. Muss in der Geschichte
            einzigartig sein.
          </dd>

          <dt>$start</dt>
          <dd>Diese Szene wird neuen Spielern als erstes
            angezeigt. Muss in der gesamten Geschichte genau einmal vorkommen.
          </dd>

          <dt>$show always<br />$always show</dt>
          <dd>
            Zeigt diese Szene immer als Option an, auch wenn die
            Vorbedingungen fehlen.
          </dd>

          <dt>$force show</dt>
          <dd>
            Zeigt diese Szene direkt als nächste an, wenn möglich, ohne dem
            Spieler eine Wahl zu lassen.
          </dd>

          <dt>$tag: <em>tag1</em> <em>tag2</em>…</dt>
          <dd>
            Füge dieser Szene Tags hinzu, um sie in der Bearebeitungsansicht zu
            einem Tag mit aufzuführen.<br />
            Kein Effekt für Spieler.
          </dd>

          <dt>$category: <em>category</em>/<em>sub</em>…</dt>
          <dd>
            Ordnet diese Szene in eine Hierarchie von Kategorien ein, so dass
            sie auf der Bearbeitungsübersicht in der entsprechenden Kategorie
            angezeigt wird.<br />
            Kein Effekt für Spieler.
          </dd>

          <dt>$set: quality = <em>expr</em></dt>
          <dd>
            Setzt die Qualität eines Spielers auf den errechneten Wert, sobald
            der Spieler diese Szene ausspielt.
          </dd>

          <dt>$require: <em>expr</em></dt>
          <dd>
            Sperrt diese Szene und versteckt sie in einer Auswahl, solange ein
            Spieler diese berechnete Bedingung nicht erfüllt.
          </dd>

          <dt>$return: <em>scene</em></dt>
          <dd>
            Bietet unter diese Szene einen "Zurück"-Knopf an, um zu der
            genannten Szene zurück zu gelangen.
          </dd>

          <dt>$continue: <em>scene</em></dt>
          <dd>
            Bietet unter diese Szene einen "Weiter"-Knopf an, um mit der
            genannten Szene fortzufahren.
          </dd>

          <dt>$include: <em>scene</em></dt>
          <dd>
            Bindet den Inhalt der genannten Szene an dieser Stelle literal ein.
            Arbeitet nicht transitiv.
          </dd>

          <dt>$option: <em>scene</em><br />…<br />$end</dt>
          <dd>
            Erstellt eine Auswahlmöglichkeit am Ende dieser Szene. Der innere
            Inhalt wird als Text der Auswahl angezeigt. Wird die Option
            ausgewählt, erscheint die genannte Szene als nächstes.
          </dd>

          <dt>$if: <em>expr</em><br />…<br />$end</dt>
          <dd>
            Zeigt Inhalt nur an, wenn eine errechnete Bedingung erfüllt ist.
          </dd>

          <dt>$if: <em>expr</em><br />…<br />$else<br />…<br />$end</dt>
          <dd>
            Zeigt Inhalt nur an, wenn eine errechnete Bedingung erfüllt ist,
            oder einen alternativen Inhalt, wenn nicht.
          </dd>


        </dl>
      </div>
    </div>
  );
}

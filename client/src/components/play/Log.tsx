import Markdown from "preact-markdown";

import Effect from "../../components/play/Effect";

import {Action, Choice} from "../../model/read/Action";
import {Quality} from "../../model/read/Quality";
import {Value} from "../../model/read/Value";
import {Property} from "../../model/read/Property";


function renderChoice(choice: Choice) {
  switch (choice.kind) {
    case "option":
      if (choice.effects.length > 0) {
        return (
          <p class="action-log py-1">
            {choice.text}
            <div class="pt-1">
              {choice.effects.map(effect => 
                <Effect {...effect} />)}
            </div>
          </p>
        );
      }
      else {
        return <p class="action-log py-1">{choice.text}</p>;
      }
    case "return": return <p class="action-log py-1">Du kehrst zurück.</p>;
    case "continue": return <hr class="w-75 mx-auto mt-2 mb-3" />;
  }
}

export default function Log(log: { entries: Array<Action> }) {
  return (
    <>
      {log.entries.map(entry => (
        <>
          <p class="markdowned">
            {Markdown(entry.text)}
          </p>
          {entry.effects.map(effect => <Effect {...effect} />)}
          {renderChoice(entry.chosen)}
        </>
      ))}
    </>
  );
}

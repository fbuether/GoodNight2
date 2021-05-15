import {Log, Choice} from "../../model/read/Log";

import Markdown from "../common/Markdown";
import Effect from "./Effect";


function Choice(choice: Choice) {
  switch (choice.kind) {
    case "action":
      if (choice.effects.length > 0) {
        return (
          <div class="action-log">
            <Markdown>{choice.text}</Markdown>
            {choice.effects.map(effect => <Effect {...effect} />)}
          </div>
        );
      }
      else {
        // <Markdown>{choice.text}</Markdown>
        return <p class="action-log py-1">{choice.text}</p>;
      }
    case "return":
      return <p class="action-log py-1">Du kehrst zurück.</p>;
    case "continue":
      return <hr class="w-75 mx-auto mt-2 mb-3" />;
  }
}


function Entry(entry: Log) {
  return (
    <>
      <Markdown>{entry.text}</Markdown>
      {entry.effects.map(Effect)}
      <Choice {...entry.chosen} />
    </>
  );
}

export default function Log(state: { entries: Array<Log> }) {
  return <>{state.entries.map(Entry)}</>;
}
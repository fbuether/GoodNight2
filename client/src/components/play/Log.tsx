import * as React from "react";
import StoryLog from "../play/StoryLog";
import ActionLog from "../play/ActionLog";


type Story = {
  type: "story";
  story: StoryLog;
};


type Action = {
  type: "action";
  action: ActionLog;
};


type Entry = Story | Action;


export default interface Log {
  entries: Array<Entry>;
}


function renderEntry(entry: Entry) {
  switch (entry.type) {
    case "story":
      return <StoryLog key={entry.story.name} {...entry.story}></StoryLog>;
    case "action":
      return <ActionLog key={entry.action.name} {...entry.action}></ActionLog>;
  }
}

export default function Log(log: Log) {

  return (
    <div>
      {log.entries.map(renderEntry)}
    </div>
  );
}

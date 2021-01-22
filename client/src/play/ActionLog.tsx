import * as React from "react";


export default interface ActionLog {
  name: string;
  text: string;
}


export default function ActionLog(log: ActionLog) {
  return (
    <p className="border-left border-lg border-light pl-2 ml-2">{log.text}</p>
  );
}

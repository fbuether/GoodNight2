import * as React from "react";


export default interface StoryLog {
  name: string;
  text: string;
}


export default function StoryLog(log: StoryLog) {
  return (
    <p>{log.text}</p>
  );
}

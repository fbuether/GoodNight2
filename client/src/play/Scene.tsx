import * as React from "react";
import Choice from "../play/Choice";


export default interface Scene {
  readonly name: string;
  readonly text: string;
  readonly choices: Array<Choice>;
}


export default function Scene(scene: Scene) {
  return (
    <div className="markdowned">
      <p>{scene.text}</p>
      <div className="list-group">
        {scene.choices.map(choice =>
          <Choice key={choice.name} {...choice}></Choice>)}
      </div>
    </div>
  );
}

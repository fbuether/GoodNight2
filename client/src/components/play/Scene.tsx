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
      <hr className="w-75 mx-auto mt-4 mb-5" />
      <div className="options list-group">
        {scene.choices.map(choice =>
          <Choice key={choice.name} {...choice}></Choice>)}
      </div>
    </div>
  );
}

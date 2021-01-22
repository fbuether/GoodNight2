import * as React from "react";
import Quality from "../play/Quality";


export default interface State {
  name: string;
  qualities: Array<Quality>;
}


export default function State(state: State) {
  return (
    <div>
      <h3>{state.name}</h3>
      <ul id="state" className="list-group">
        {state.qualities.map(quality =>
          <Quality key={quality.name} {...quality}></Quality>)}
      </ul>
    </div>
  );
}

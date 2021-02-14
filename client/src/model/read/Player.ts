import {requireString, deserialiseArray} from "../Deserialise";

import {Property, asProperty} from "./Property";

export interface Player {
  readonly user: string;
  readonly name: string;
  readonly state: Array<Property>;
}


export function asPlayer(obj: any): Player {
  return {
    user: requireString(obj["user"]),
    name: requireString(obj["name"]),
    state: deserialiseArray(obj, "state", asProperty)
  };
}

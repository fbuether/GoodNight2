import {requireString, requireBoolean, optionalString,
  deserialiseArray} from "../Deserialise";

import {Quality, asQuality} from "./Quality";
import {Value, asValue} from "./Value";
import {Property, asProperty} from "./Property";


export interface Requirement {
  description: string;
  passed: boolean;
}


export interface Option {
  scene: string;
  isAvailable: boolean;
  text: string;
  requirements: Array<Requirement>;
}


export interface Scene {
  name: string;
  text: string;
  effects: Array<Property>;
  options: Array<Option>;
  return?: string;
  continue?: string;
}


function asRequirement(obj: any): Requirement {
  return {
    description: requireString(obj["description"]),
    passed: requireBoolean(obj["passed"])
  };
}

function asOption(obj: any): Option {
  return {
    scene: requireString(obj["scene"]),
    isAvailable: requireBoolean(obj["isAvailable"]),
    text: requireString(obj["text"]),
    requirements: deserialiseArray(obj, "requirements", asRequirement)
  };
}

export function asScene(obj: any): Scene {
  return {
    name: requireString(obj["name"]),
    text: requireString(obj["text"]),
    effects: deserialiseArray(obj, "effects", asProperty),
    options: deserialiseArray(obj, "options", asOption),
    return: optionalString(obj, "return"),
    continue: optionalString(obj, "continue")
  };
}

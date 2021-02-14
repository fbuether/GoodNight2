import {Value, asValue} from "./Value";
import {Quality, asQuality} from "./Quality";


export interface Property {
  quality: Quality;
  value: Value;
}


export function asProperty(obj: any): Property {
  return {
    quality: asQuality(obj["quality"]),
    value: asValue(obj["value"])
  };
}

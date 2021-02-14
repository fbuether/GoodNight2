import {requireString, requireNumber, optionalString} from "../Deserialise";


export enum QualityType {
  Int = 1,
  Bool = 2,
  Enum = 3
}

export interface Quality {
  readonly name: string;
  readonly type: QualityType;
  readonly description: string;
  readonly scene?: string;
}


export function asQuality(obj: any): Quality {
  let typeNum = requireNumber(obj["type"]);
  if (typeNum < 1 || typeNum > 3) {
    console.error("Invalid QualityType", typeNum);
    throw new Error("Invalid QualityType");
  }

  return {
    name: requireString(obj["name"]),
    type: typeNum as QualityType,
    description: requireString(obj["description"]),
    scene: optionalString(obj, "scene")
  };
}

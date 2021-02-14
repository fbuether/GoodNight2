import {requireString, requireNumber, requireBoolean} from "../Deserialise";

interface BoolValue {
  readonly kind: "bool";
  readonly value: boolean;
}

interface IntValue {
  readonly kind: "int";
  readonly value: number;
}

interface EnumValue {
  readonly kind: "enum";
  readonly value: number;
}

export type Value = BoolValue | IntValue | EnumValue;


export function asValue(obj: any): Value {
  let kind = requireString(obj["kind"]);
  switch (kind) {
    case "Bool":
      return {
        kind: "bool",
        value: requireBoolean(obj["value"])
      };
    case "Int":
    case "Enum":
      return {
        kind: kind == "Enum" ? "enum" : "int",
        value: requireNumber(obj["value"])
      };
    default:
      console.error("Invalid value kind", kind);
      throw new Error("Invalid value kind");
  }
}

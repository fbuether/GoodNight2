
interface BoolValue {
  kind: "bool";
  value: boolean;
}

interface IntValue {
  kind: "int";
  value: number;
}

interface EnumValue {
  kind: "enum";
  value: number;
}

export type Value = BoolValue | IntValue | EnumValue;
